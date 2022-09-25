#if NET35 || NET40
using System.Threading;
#else
using System.Threading.Tasks;
#endif
#if NET35 || NET40
using Timer = System.Timers.Timer;
#else
using System.Timers;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using G9ScheduleManagement.Enum;
using G9ScheduleManagement.G9ScheduleItem;

namespace G9ScheduleManagement
{
    /// <summary>
    ///     A pretty small library for working with schedulers
    /// </summary>
    public partial class G9Scheduler
    {
        private DateTime _currentDateTimeForEachRound;
        private IEnumerable<G9DtScheduler> _mainQueryForSchedulers;

        /// <summary>
        ///     Static Constructor - Initialize Requirements
        /// </summary>
        static G9Scheduler()
        {
            // Set event for exiting or unloading
            AppDomain.CurrentDomain.ProcessExit +=
                (sender, args) => OnApplicationStop(G9EDisposeReason.DisposedOnApplicationStop);
            AppDomain.CurrentDomain.DomainUnload += (sender, args) =>
                OnApplicationStop(G9EDisposeReason.DisposedOnApplicationStop);
        }

        /// <summary>
        ///     Constructor - Initialize Requirements
        /// </summary>
        public G9Scheduler()
        {
            if (ScheduleIdentity != 0) return;
            Initialize(true);
            SchedulerState = G9ESchedulerState.InitializedState;
        }

        /// <summary>
        ///     Constructor for accessing a scheduler by its identity.
        /// </summary>
        /// <param name="scheduleIdentity">Specifies the unique identity of a scheduler.</param>
        public G9Scheduler(uint scheduleIdentity)
        {
            if (!SchedulersCollection.ContainsKey(scheduleIdentity))
                throw new Exception($"The scheduler by this unique identity '{scheduleIdentity}' was not found.");
            ScheduleIdentity = scheduleIdentity;
            lock (LockCollectionForScheduleTask)
            {
                _scheduler = SchedulersCollection[ScheduleIdentity];
            }

            Initialize(false);
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            lock (LockCollectionForScheduleTask)
            {
                SchedulersCollection.Remove(ScheduleIdentity);
            }

            ScheduleIdentity = 0;
            if (_scheduler.DisposeCallbacks != null)
                foreach (var action in _scheduler.DisposeCallbacks)
                    action?.Invoke(this, G9EDisposeReason.DisposedByMethod);
            _scheduler = null;
        }

        /// <summary>
        ///     Method to initialize requirements.
        /// </summary>
        /// <param name="addNewSchedule">Specifies whether a new scheduler must be added or not.</param>
        private void Initialize(bool addNewSchedule)
        {
            if (addNewSchedule)
                lock (LockCollectionForScheduleTask)
                {
                    ScheduleIdentity = _scheduleIdentityIndex++;
                    _scheduler = new G9DtScheduler(this);
                    // Add schedule
                    SchedulersCollection.Add(ScheduleIdentity, _scheduler);
                }

            if (_mainTimer != null) return;

            lock (LockCollectionForScheduleTask)
            {
                if (_mainTimer != null) return;

                _currentDateTimeForEachRound = DateTime.Now;
                _mainQueryForSchedulers = SchedulersCollection
                    .Where(s =>
                        s.Value.SchedulerState != G9ESchedulerState.PausedState &&
                        s.Value.SchedulerState != G9ESchedulerState.InitializedState &&
                        s.Value.SchedulerState != G9ESchedulerState.FinishedState &&
                        s.Value.SchedulerActions != null &&
                        (!s.Value.HasStartDateTime || s.Value.StartDateTime <= _currentDateTimeForEachRound) &&
                        (!s.Value.HasStartTime || s.Value.StartTime <= _currentDateTimeForEachRound.TimeOfDay) &&
                        (!s.Value.HasEndTime || s.Value.EndTime >= _currentDateTimeForEachRound.TimeOfDay) &&
                        // Check queue
                        (!s.Value.IsSchedulerQueueEnable ||
                         s.Value.SchedulerState != G9ESchedulerState.StartedStateOnPreExecution)
                    )
                    .Select(s => s.Value);

                _mainTimer = new Timer(1);
                _mainTimer.Elapsed += (sender, args) =>
                {
                    if (_cancelToken)
                    {
                        _mainTimer.Stop();
                        _mainTimer.Dispose();
                    }

                    if (!_isSchedulerRoundIsFinished) return;
                    _isSchedulerRoundIsFinished = false;
                    ScheduleHandler();
                };
                _mainTimer.Start();
            }
        }


        /// <summary>
        ///     Helper method to handle scheduled items
        /// </summary>
        private void ScheduleHandler()
        {
            try
            {
                _currentDateTimeForEachRound = DateTime.Now;

                G9DtScheduler[] scheduleList;
                lock (LockCollectionForScheduleTask)
                {
                    scheduleList = _mainQueryForSchedulers
                        .ToArray();
                }
#if NET35 || NET40
                foreach (var scheduledItem in scheduleList)
                    ScheduleItemHandler(scheduledItem, _currentDateTimeForEachRound);
#else
                Parallel.ForEach(scheduleList, scheduledItem =>
                    ScheduleItemHandler(scheduledItem, _currentDateTimeForEachRound));
#endif
            }
            finally
            {
                _isSchedulerRoundIsFinished = true;
            }
        }

        /// <summary>
        ///     Helper method to handle a scheduled item
        /// </summary>
        /// <param name="scheduledItem">Specified a schedule item</param>
        /// <param name="currentDateTime">Specified current date time per each round</param>
        private static void ScheduleItemHandler(G9DtScheduler scheduledItem, DateTime currentDateTime)
        {
            // Save the starter state
            var startedState = scheduledItem.SchedulerState;

            // Set pre execution status
            scheduledItem.SchedulerState = G9ESchedulerState.StartedStateOnPreExecution;

#if NET35 || NET40
            var thread = new Thread(() =>
            {
#else
            Task.Run(() =>
            {
#endif
                try
                {
                    // Run pre-execution callbacks
                    CollectionRunnerHelper(scheduledItem.PreExecutionCallbacks, scheduledItem.Scheduler);

                    // Checks the condition of the count of repetitions.
                    var isTryingRound = false;
                    if (scheduledItem.HasCustomCountOfRepetitions)
                    {
                        // Checks the condition of the count of tries
                        if (startedState == G9ESchedulerState.HasError &&
                            scheduledItem.HasCustomCountOfRepetitions && scheduledItem.HasCustomCountOfTries &&
                            scheduledItem.CountOfTriesCounter < scheduledItem.CountOfTries)
                        {
                            if (currentDateTime - scheduledItem.LastRunDateTime < scheduledItem.GapBetweenEachTry)
                                return;

                            isTryingRound = true;
                            unchecked
                            {
                                scheduledItem.CountOfTriesCounter++;
                            }
                        }
                        else
                        {
                            // The counter must reset daily if the checking type was set to per day.
                            if (scheduledItem.RepetitionConditionType == G9ERepetitionConditionType.PerDay &&
                                scheduledItem.RepetitionsDateTime.Day != DateTime.Now.Day)
                            {
                                scheduledItem.RepetitionsDateTime = DateTime.Now;
                                scheduledItem.CountOfRepetitionsCounter = 0;
                            }

                            if (scheduledItem.CountOfRepetitionsCounter >=
                                scheduledItem.CountOfRepetitions)
                            {
                                // The repetition condition can finish the scheduler when the repetition type isn't per day.
                                if (scheduledItem.RepetitionConditionType == G9ERepetitionConditionType.InTotal)
                                    FinishingHelper(scheduledItem, G9EFinishingReason.FinishedByRepetitionCondition,
                                        null);

                                return;
                            }
                        }
                    }

                    // Checks the condition of the end date time.
                    // Unlike the start date time, the end date time must be checked inner of the method
                    // because if the current date time passes to the end date time, the scheduled item must be finished.
                    if (!isTryingRound && scheduledItem.HasEndDateTime &&
                        scheduledItem.EndDateTime < currentDateTime)
                    {
                        FinishingHelper(scheduledItem, G9EFinishingReason.FinishedByEndDateTimeCondition, null);
                        return;
                    }

                    // Checks the condition of the period duration between each execution.
                    if (!isTryingRound && scheduledItem.HasCustomPeriodDuration &&
                        currentDateTime - scheduledItem.LastRunDateTime <
                        scheduledItem.CustomPeriodDuration)
                        return;

                    // Checks the condition of specified custom conditions.
                    if (!isTryingRound && scheduledItem.ConditionFunctions != null &&
                        scheduledItem.ConditionFunctions.Any(s => !s(scheduledItem.Scheduler)))
                        return;

                    // Set the last execution date time for this scheduler item
                    scheduledItem.LastRunDateTime = currentDateTime;

                    // Run scheduler actions
                    CollectionRunnerHelper(scheduledItem.SchedulerActions, scheduledItem.Scheduler);

                    // The scheduler action is running if all former conditions are checked and passed.
                    scheduledItem.SchedulerState = G9ESchedulerState.StartedStateOnEndExecution;

                    // Adds one number to the period counter during each run.
                    unchecked
                    {
                        scheduledItem.CountOfRepetitionsCounter++;
                    }

                    // Reset the tries counter
                    scheduledItem.CountOfTriesCounter = 0;

                    // Run pre-execution callbacks
                    CollectionRunnerHelper(scheduledItem.EndExecutionCallbacks, scheduledItem.Scheduler);
                }
                catch (Exception ex)
                {
                    scheduledItem.SchedulerState = G9ESchedulerState.HasError;
                    if (scheduledItem.ErrorCallbacks != null)
                        foreach (var action in scheduledItem.ErrorCallbacks)
                            try
                            {
                                action?.Invoke(scheduledItem.Scheduler, ex);
                            }
                            catch
                            {
                                // Ignore
                            }
                }
                finally
                {
                    if (scheduledItem.SchedulerState == G9ESchedulerState.StartedStateOnPreExecution)
                        scheduledItem.SchedulerState = G9ESchedulerState.ConditionalRejectExecution;
                }

#if NET35 || NET40
            })
            {
                IsBackground = true
            };
            thread.Start();
#else
            });
#endif
        }

        /// <summary>
        ///     Helper method to handle the situation on application stop or exit
        /// </summary>
        private static void OnApplicationStop(G9EDisposeReason reason)
        {
            _cancelToken = true;

            lock (LockCollectionForScheduleTask)
            {
                foreach (var scheduler in SchedulersCollection
                             .Where(scheduler => scheduler.Value.DisposeCallbacks != null).Select(s => s.Value))
                foreach (var schedulerDisposeCallback in scheduler.DisposeCallbacks)
                    try
                    {
                        schedulerDisposeCallback?.Invoke(scheduler.Scheduler, reason);
                    }
                    catch
                    {
                        // Ignore
                    }
            }
        }

        /// <summary>
        ///     Check validation
        /// </summary>
        private void CheckValidation()
        {
            if (_scheduler == null) throw new Exception("The scheduler was already disposed.");
        }

        /// <summary>
        ///     Method to remove a custom item from a HashSet collection
        ///     <para />
        ///     Generate readable exception error if occurs.
        /// </summary>
        private void CollectionRemoveHelper<TType>(ref HashSet<TType> category, TType customItem, string parameterName)
        {
            if (category == null)
                throw new ArgumentException(
                    $"No items have been added to this scheduler, so you cannot remove a specified action (here is '{parameterName}') from it.",
                    parameterName);
            if (!category.Remove(customItem))
                throw new ArgumentException(
                    $"No items have been added to this scheduler with this structure, so you cannot remove a specified action (here is '{parameterName}') from it.",
                    parameterName);

            if (category.Count == 0)
                category = null;
        }

        /// <summary>
        ///     Method to add a custom item to HashSet collection
        /// </summary>
        private void CollectionAdderHelper<TType>(ref HashSet<TType> category, TType customItem, string parameterName)
        {
            if (customItem == null)
                throw new ArgumentException(
                    $"The specified item for adding cannot be null (here is '{parameterName}').", parameterName);
            if (category == null)
                category = new HashSet<TType>();
            category.Add(customItem);
        }

        /// <summary>
        ///     Method to execute a collection of actions
        /// </summary>
        private static void CollectionRunnerHelper(HashSet<Action<G9Scheduler>> category, G9Scheduler schedulerObject,
            bool ignoreException = false)
        {
            if (ignoreException)
            {
                if (category == null) return;
                foreach (var action in category)
                    try
                    {
                        action?.Invoke(schedulerObject);
                    }
                    catch
                    {
                        // Ignore
                    }
            }
            else
            {
                if (category == null) return;
                foreach (var action in category)
                    action?.Invoke(schedulerObject);
            }
        }

        /// <summary>
        ///     Helper method to handle scheduler items that were finished.
        /// </summary>
        private static void FinishingHelper(G9DtScheduler scheduledItem, G9EFinishingReason finishingReason,
            string information)
        {
            try
            {
                scheduledItem.SchedulerState = G9ESchedulerState.FinishedState;
                // Run all finish call back
                if (scheduledItem.FinishCallbacks != null)
                    foreach (var actions in scheduledItem.FinishCallbacks)
                        actions?.Invoke(scheduledItem.Scheduler, finishingReason, information);
            }
            catch (Exception ex)
            {
                foreach (var action in scheduledItem.ErrorCallbacks)
                    try
                    {
                        action?.Invoke(scheduledItem.Scheduler, ex);
                    }
                    catch
                    {
                        // Ignore
                    }
            }
        }
    }
}