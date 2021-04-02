using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using G9ScheduleManagement.G9ScheduleItem;

namespace G9ScheduleManagement
{
    /// <summary>
    ///     Class managed schedule task
    /// </summary>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2
    [Serializable]
#endif
    public class G9CSchedule : IDisposable
    {
        #region Fields And Properties

        /// <summary>
        ///     Lock collection for use
        /// </summary>
        private static readonly object LockCollectionForScheduleTask = new object();

        /// <summary>
        ///     Field Sorted dictionary for save Schedule items
        ///     Use for lock
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static SortedDictionary<Guid, G9DtScheduleItem> _saveScheduleTask =
            new SortedDictionary<Guid, G9DtScheduleItem>();

        /// <summary>
        ///     Specify enable Schedule count
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static int ScheduleItemCount
        {
            get
            {
                lock (LockCollectionForScheduleTask)
                {
                    return _saveScheduleTask.Count;
                }
            }
        }

        /// <summary>
        ///     Save schedule for this class object
        /// </summary>
        private G9DtScheduleItem _scheduleItem;

        /// <summary>
        ///     Unique identity for schedule
        /// </summary>
        public Guid ScheduleIdentity { private set; get; }

        /// <summary>
        ///     Specify Schedule is active
        /// </summary>
        private static bool _activeSchedule;

        /// <summary>
        ///     Specify method Schedule handler finished or no
        /// </summary>
        private static bool _waitForFinishScheduleHandler;

        /// <summary>
        ///     Specify number of execution for this Schedule
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int NumberOfExecution => _scheduleItem.PeriodCounter;

        /// <summary>
        ///     Cancellation Token For Dispose
        /// </summary>
        private static CancellationToken _cancellationToken;

        private static CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        /// <summary>
        ///     Initialize Requirement
        ///     Set timer
        /// </summary>
        /// <param name="addNewSchedule">Specify add new Schedule</param>
        private void Initialize(bool addNewSchedule)
        {
            if (addNewSchedule)
            {
                // Set Identity
                _scheduleItem = new G9DtScheduleItem {Identity = ScheduleIdentity = Guid.NewGuid()};

                lock (LockCollectionForScheduleTask)
                {
                    // Add schedule
                    _saveScheduleTask.Add(ScheduleIdentity, _scheduleItem);
                }
            }

            // If use cancellation token - initialize again
            if (_cancellationToken.IsCancellationRequested)
                InitializeCancellationTokens();

            // if not active return
            if (_activeSchedule) return;

            _activeSchedule = true;
            Task.Factory.StartNew(async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                    // Check Schedule handler method finished or no
                    if (_waitForFinishScheduleHandler)
                    {
                        // if not finished Delay and check again
                        await Task.Delay(1);
                    }
                    else
                    {
                        // if finished run Schedule handler again and wait
                        try
                        {
                            await ScheduleHandler();
                        }
                        catch
                        {
                            // Ignore
                        }

                        await Task.Delay(1);
                    }

                // ReSharper disable once FunctionNeverReturns
            }, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        /// <summary>
        ///     Constructor
        ///     Initialize Requirement
        /// </summary>
        public G9CSchedule()
        {
            Initialize(true);
        }

        /// <summary>
        ///     Static Constructor
        ///     Initialize Requirement
        /// </summary>
        static G9CSchedule()
        {
            InitializeCancellationTokens();
        }

        /// <summary>
        ///     Initialize Cancellation Tokens
        /// </summary>
        private static void InitializeCancellationTokens()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        /// <summary>
        ///     Constructor - Restore Schedule by identity
        ///     Initialize Requirement
        /// </summary>
        /// <param name="scheduleIdentity">Specify Schedule identity for restore enable Schedule</param>
        public G9CSchedule(Guid scheduleIdentity)
        {
            lock (LockCollectionForScheduleTask)
            {
                if (!_saveScheduleTask.ContainsKey(scheduleIdentity))
                    throw new Exception("Schedule not found!");

                _scheduleItem = _saveScheduleTask[scheduleIdentity];
            }

            ScheduleIdentity = scheduleIdentity;

            Initialize(false);
        }

        /// <summary>
        ///     Dispose this Schedule
        /// </summary>
        public void Dispose()
        {
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].DisposeCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].DisposeCallBack = new HashSet<Action>();
                foreach (var action in _saveScheduleTask[ScheduleIdentity].DisposeCallBack) action?.Invoke();
                _saveScheduleTask.Remove(ScheduleIdentity);
            }

            ScheduleIdentity = Guid.Empty;
            _scheduleItem = null;

            // Dispose schedule system if not exist any task
            if (!_saveScheduleTask.Any()) _cancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Check validation
        /// </summary>
        private void CheckValidation()
        {
            if (_scheduleItem == null) throw new Exception("Object Disposed!");
        }

        /// <summary>
        ///     Add action for Schedule
        /// </summary>
        /// <param name="scheduleAction">Action for Schedule</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule AddScheduleAction(Action scheduleAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].ScheduleAction == null)
                    _saveScheduleTask[ScheduleIdentity].ScheduleAction = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].ScheduleAction.Add(scheduleAction);
            }

            return this;
        }

        /// <summary>
        ///     Remove action from Schedule
        /// </summary>
        /// <param name="scheduleAction">Action for remove</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule RemoveScheduleAction(Action scheduleAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].ScheduleAction == null)
                    _saveScheduleTask[ScheduleIdentity].ScheduleAction = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].ScheduleAction.Remove(scheduleAction);
            }

            return this;
        }

        /// <summary>
        ///     Add finish call back for Schedule
        /// </summary>
        /// <param name="finishCallBackAction">Action for finish call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule AddFinishCallBack(Action finishCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].FinishCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].FinishCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].FinishCallBack.Add(finishCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Remove finish call back for Schedule
        /// </summary>
        /// <param name="finishCallBackAction">Action for remove finish call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule RemoveFinishCallBack(Action finishCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].FinishCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].FinishCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].FinishCallBack.Remove(finishCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Add stop call back for Schedule
        /// </summary>
        /// <param name="stopCallBackAction">Action for stop call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule AddStopCallBack(Action stopCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].StopCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].StopCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].StopCallBack.Add(stopCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Remove stop call back for Schedule
        /// </summary>
        /// <param name="stopCallBackAction">Action for remove stop call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule RemoveStopCallBack(Action stopCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].StopCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].StopCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].StopCallBack.Remove(stopCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Add resume call back for Schedule
        /// </summary>
        /// <param name="resumeCallBackAction">Action for resume call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule AddResumeCallBack(Action resumeCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].ResumeCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].ResumeCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].ResumeCallBack.Add(resumeCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Remove resume call back for Schedule
        /// </summary>
        /// <param name="resumeCallBackAction">Action for remove resume call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule RemoveResumeCallBack(Action resumeCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].ResumeCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].ResumeCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].ResumeCallBack.Remove(resumeCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Add Error call back for Schedule
        /// </summary>
        /// <param name="errorCallBackAction">Action for error call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule AddErrorCallBack(Action<Exception> errorCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].ErrorCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].ErrorCallBack = new HashSet<Action<Exception>>();
                _saveScheduleTask[ScheduleIdentity].ErrorCallBack.Add(errorCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Remove Error call back for Schedule
        /// </summary>
        /// <param name="errorCallBackAction">Action for remove error call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule RemoveErrorCallBack(Action<Exception> errorCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].ErrorCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].ErrorCallBack = new HashSet<Action<Exception>>();
                _saveScheduleTask[ScheduleIdentity].ErrorCallBack.Remove(errorCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Add dispose call back for Schedule
        /// </summary>
        /// <param name="disposeCallBackAction">Action for dispose call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule AddDisposeCallBack(Action disposeCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].DisposeCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].DisposeCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].DisposeCallBack.Add(disposeCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Remove dispose call back for Schedule
        /// </summary>
        /// <param name="disposeCallBackAction">Action for remove dispose call back</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule RemoveDisposeCallBack(Action disposeCallBackAction)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].DisposeCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].DisposeCallBack = new HashSet<Action>();
                _saveScheduleTask[ScheduleIdentity].DisposeCallBack.Remove(disposeCallBackAction);
            }

            return this;
        }

        /// <summary>
        ///     Set start date time for Schedule
        ///     Add filter for Schedule => Specify start date time for Schedule
        /// </summary>
        /// <param name="startDateTime">Specify start date time</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule SetStartDateTime(DateTime startDateTime)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                _saveScheduleTask[ScheduleIdentity].StartDateTime = startDateTime;
            }

            return this;
        }

        /// <summary>
        ///     Set start date time for Schedule
        ///     Add filter for Schedule => Specify finish date time for Schedule
        /// </summary>
        /// <param name="finishDateTime">Specify finish date time</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule SetFinishDateTime(DateTime finishDateTime)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                _saveScheduleTask[ScheduleIdentity].FinishDateTime = finishDateTime;
            }

            return this;
        }

        /// <summary>
        ///     Set start date time for Schedule
        /// </summary>
        /// <param name="duration">Specify duration for run Schedule action task</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule SetDuration(TimeSpan duration)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                _saveScheduleTask[ScheduleIdentity].Duration = duration;
            }

            return this;
        }

        /// <summary>
        ///     Set Schedule period
        ///     Add filter for Schedule => Number of execution for Schedule
        /// </summary>
        /// <param name="period">Specify number of execution for this Schedule</param>
        /// <returns>Return G9Scheduled</returns>
        public G9CSchedule SetPeriod(int period)
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                _saveScheduleTask[ScheduleIdentity].Period = period;
            }

            return this;
        }

        /// <summary>
        ///     Handler for Schedule
        ///     run every time and execute schedule action task
        /// </summary>
        private static async Task ScheduleHandler()
        {
            var oTask = Task.Run(() =>
            {
                _waitForFinishScheduleHandler = true;
                var currentDateTime = DateTime.Now;
                List<KeyValuePair<Guid, G9DtScheduleItem>> deletedList = null;
                List<KeyValuePair<Guid, G9DtScheduleItem>> scheduleList;
                try
                {
                    lock (LockCollectionForScheduleTask)
                    {
                        scheduleList = _saveScheduleTask.ToList();
                    }

                    foreach (var scheduledItem in scheduleList)
                        try
                        {
                            // If Schedule disable => stop
                            if (!scheduledItem.Value.EnableSchedule)
                                continue;

                            // If Schedule period enable and greater than PeriodCounter => remove from category and continue
                            if (scheduledItem.Value.Period > 0 &&
                                scheduledItem.Value.PeriodCounter >= scheduledItem.Value.Period)
                            {
                                if (deletedList == null)
                                    deletedList = new List<KeyValuePair<Guid, G9DtScheduleItem>>();
                                deletedList.Add(scheduledItem);
                                continue;
                            }

                            // If action is null continue
                            if (scheduledItem.Value.ScheduleAction == null ||
                                scheduledItem.Value.Duration == Timeout.InfiniteTimeSpan)
                                continue;

                            // Check start time
                            if (scheduledItem.Value.StartDateTime != DateTime.MinValue &&
                                scheduledItem.Value.StartDateTime > currentDateTime)
                                continue;

                            // Check end date time => remove from category and continue if EndTime < DateTime.Now
                            if (scheduledItem.Value.FinishDateTime != DateTime.MinValue &&
                                scheduledItem.Value.FinishDateTime < currentDateTime)
                            {
                                if (deletedList == null)
                                    deletedList = new List<KeyValuePair<Guid, G9DtScheduleItem>>();
                                deletedList.Add(scheduledItem);
                                continue;
                            }

                            // Check duration
                            if (currentDateTime - scheduledItem.Value.LastRunDateTime <
                                scheduledItem.Value.Duration)
                                continue;

                            // Run job
                            foreach (var action in scheduledItem.Value.ScheduleAction) action?.Invoke();

                            // plus period counter when run
                            scheduledItem.Value.PeriodCounter++;

                            // Set last run date time
                            scheduledItem.Value.LastRunDateTime = currentDateTime;
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                foreach (var action in scheduledItem.Value.ErrorCallBack) action?.Invoke(ex);
                            }
                            catch
                            {
                                // Ignore
                            }
                        }
                }
                finally
                {
                    // Delete items in list
                    if (deletedList != null)
                        foreach (var deleteItem in deletedList)
                            RemoveScheduleItem(deleteItem);

                    // wait flag
                    _waitForFinishScheduleHandler = false;
                }
            });
            await oTask;
        }

        /// <summary>
        ///     Remove Schedule item from category
        ///     Helper method
        /// </summary>
        /// <param name="g9ScheduledItem">Specify Schedule item</param>
        private static void RemoveScheduleItem(KeyValuePair<Guid, G9DtScheduleItem> g9ScheduledItem)
        {
            // remove from schedule
            try
            {
                // Run all finish call back
                foreach (var actions in g9ScheduledItem.Value.FinishCallBack)
                    actions?.Invoke();
            }
            catch (Exception ex)
            {
                foreach (var action in g9ScheduledItem.Value.ErrorCallBack) action?.Invoke(ex);
            }
            finally
            {
                try
                {
                    lock (LockCollectionForScheduleTask)
                    {
                        // Remove
                        _saveScheduleTask.Remove(g9ScheduledItem.Key);
                    }
                }
                catch
                {
                    // Ignore
                }
            }
        }

        /// <summary>
        ///     Resume Schedule if stop
        /// </summary>
        public void Resume()
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (_saveScheduleTask[ScheduleIdentity].EnableSchedule)
                    return;
                if (_saveScheduleTask[ScheduleIdentity].ResumeCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].ResumeCallBack = new HashSet<Action>();
                foreach (var action in _saveScheduleTask[ScheduleIdentity].ResumeCallBack) action?.Invoke();
                _saveScheduleTask[ScheduleIdentity].EnableSchedule = true;
            }
        }

        /// <summary>
        ///     Stop Schedule
        /// </summary>
        public void Stop()
        {
            CheckValidation();
            lock (LockCollectionForScheduleTask)
            {
                if (!_saveScheduleTask[ScheduleIdentity].EnableSchedule)
                    return;
                _saveScheduleTask[ScheduleIdentity].EnableSchedule = false;
                if (_saveScheduleTask[ScheduleIdentity].StopCallBack == null)
                    _saveScheduleTask[ScheduleIdentity].StopCallBack = new HashSet<Action>();
                foreach (var action in _saveScheduleTask[ScheduleIdentity].StopCallBack) action?.Invoke();
            }
        }

        /// <summary>
        ///     Reset duration for current Schedule
        /// </summary>
        public void ResetDuration()
        {
            lock (LockCollectionForScheduleTask)
            {
                _saveScheduleTask[ScheduleIdentity].LastRunDateTime = DateTime.Now;
            }
        }

        #endregion
    }
}