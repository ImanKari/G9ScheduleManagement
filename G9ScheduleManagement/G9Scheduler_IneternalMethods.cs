#if NET35 || NET40
using System.Threading;
#else
using System.Threading.Tasks;
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

        /// <inheritdoc />
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

            if (_mainTask != null) return;

            lock (LockCollectionForScheduleTask)
            {
                if (_mainTask != null) return;
#if NET35 || NET40
                _mainTask = new Thread(ScheduleHandler)
                {
                    IsBackground = true
                };
                _mainTask.Start();
#else
                _mainTask = Task.Factory.StartNew(async () => await ScheduleHandler(),
                    CancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
#endif
            }
        }

        /// <summary>
        ///     Helper method to handle scheduled items
        /// </summary>
#if NET35 || NET40
        private void ScheduleHandler()
#else
        private async Task ScheduleHandler()
#endif
        {
#if NET35 || NET40
            while (!_cancelToken)
            {
#else
            var task = Task.Factory.StartNew(async () =>
            {
                while (!CancelToken.IsCancellationRequested)
                {
#endif


                    var currentDateTime = DateTime.Now;
                    var query = SchedulersCollection
                        .Where(s =>
                            s.Value.SchedulerState != G9ESchedulerState.PausedState &&
                            s.Value.SchedulerState != G9ESchedulerState.InitializedState &&
                            s.Value.SchedulerState != G9ESchedulerState.FinishedState &&
                            s.Value.SchedulerActions != null &&
                            (!s.Value.HasStartDateTime || s.Value.StartDateTime <= currentDateTime) &&
                            (!s.Value.HasStartTime || s.Value.StartTime <= currentDateTime.TimeOfDay) &&
                            (!s.Value.HasEndTime || s.Value.EndTime >= currentDateTime.TimeOfDay) &&
                            // Check queue
                            (!s.Value.IsSchedulerQueueEnable ||
                             s.Value.SchedulerState != G9ESchedulerState.StartedStateOnPreExecution)
                        )
                        .Select(s => s.Value);

                    G9DtScheduler[] scheduleList;
                    lock (LockCollectionForScheduleTask)
                    {
                        scheduleList = query
                            .ToArray();
                    }
#if NET35 || NET40
                foreach (var scheduledItem in scheduleList)
                    new Thread(o => { ScheduleItemHandler(scheduledItem, currentDateTime); })
                    {
                        IsBackground = true
                    }.Start();
                Thread.Sleep(1);
#else
                    Parallel.ForEach(scheduleList, schedulerAction =>
                        Task.Run(() => ScheduleItemHandler(schedulerAction, currentDateTime))
                    );
                    await Task.Delay(1);
#endif

#if NET35 || NET40
            }
#else
                }
            }, CancelToken.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
            await task;
#endif
        }

        /// <summary>
        ///     Helper method to handle a scheduled item
        /// </summary>
        /// <param name="scheduledItem">Specified a schedule item</param>
        /// <param name="currentDateTime">Specified current date time per each round</param>
        private static void ScheduleItemHandler(G9DtScheduler scheduledItem, DateTime currentDateTime)
        {
            try
            {
                // Set pre execution status
                scheduledItem.SchedulerState = G9ESchedulerState.StartedStateOnPreExecution;

                // Run pre-execution callbacks
                CollectionRunnerHelper(scheduledItem.PreExecutionCallbacks, scheduledItem.Scheduler);

                // Checks the condition of the count of repetitions.
                if (scheduledItem.HasCustomCountOfRepetitions)
                {
                    // The counter must reset daily if the checking type was set to per day.
                    if (scheduledItem.RepetitionConditionType == G9ERepetitionConditionType.PerDay &&
                        scheduledItem.CountOfRepetitionsDateTime.Day != DateTime.Now.Day)
                    {
                        scheduledItem.CountOfRepetitionsDateTime = DateTime.Now;
                        scheduledItem.CountOfRepetitionsCounter = 0;
                    }

                    if (scheduledItem.CountOfRepetitionsCounter >=
                        scheduledItem.CountOfRepetitions)
                    {
                        // The repetition condition can finish the scheduler when the repetition type isn't per day.
                        if (scheduledItem.RepetitionConditionType == G9ERepetitionConditionType.InTotal)
                            FinishingHelper(scheduledItem, G9EFinishingReason.FinishedByRepetitionCondition, null);

                        return;
                    }
                }

                // Checks the condition of the end date time.
                // Unlike the start date time, the end date time must be checked inner of the method
                // because if the current date time passes to the end date time, the scheduled item must be finished.
                if (scheduledItem.HasEndDateTime &&
                    scheduledItem.EndDateTime < currentDateTime)
                {
                    FinishingHelper(scheduledItem, G9EFinishingReason.FinishedByEndDateTimeCondition, null);
                    return;
                }

                // Checks the condition of the period duration between each execution.
                if (scheduledItem.HasCustomPeriodDuration && currentDateTime - scheduledItem.LastRunDateTime <
                    scheduledItem.CustomPeriodDuration)
                {
                    scheduledItem.SchedulerState = G9ESchedulerState.StartedStateOnEndExecution;
                    return;
                }

                // Checks the condition of specified custom conditions.
                if (scheduledItem.ConditionFunctions != null &&
                    scheduledItem.ConditionFunctions.Any(s => !s(scheduledItem.Scheduler)))
                {
                    scheduledItem.SchedulerState = G9ESchedulerState.StartedStateOnEndExecution;
                    return;
                }

                // Run scheduler actions
                CollectionRunnerHelper(scheduledItem.SchedulerActions, scheduledItem.Scheduler);

                // Run pre-execution callbacks
                CollectionRunnerHelper(scheduledItem.EndExecutionCallbacks, scheduledItem.Scheduler);

                // The scheduler action is running if all former conditions are checked and passed.
                scheduledItem.SchedulerState = G9ESchedulerState.StartedStateOnEndExecution;

                // Adds one number to the period counter during each run.
                unchecked
                {
                    scheduledItem.CountOfRepetitionsCounter++;
                }

                // Set the last execution date time for this scheduler item
                scheduledItem.LastRunDateTime = currentDateTime;
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
        }

        /// <summary>
        ///     Helper method to handle the situation on application stop or exit
        /// </summary>
        private static void OnApplicationStop(G9EDisposeReason reason)
        {
#if NET35 || NET40
            _cancelToken = false;
#else
            CancelToken.Cancel();
#endif

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
        private static void FinishingHelper(G9DtScheduler scheduledItem, G9EFinishingReason finishingReason, string information)
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