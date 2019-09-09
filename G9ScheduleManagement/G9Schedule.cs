using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace G9ScheduleManagement
{
    /// <summary>
    ///     Class managed schedule task
    /// </summary>
    public class G9Schedule : IDisposable
    {
        #region Fields And Properties

        /// <summary>
        ///     Field Sorted dictionary for save Schedule items
        ///     Use for lock
        /// </summary>
        private static SortedDictionary<Guid, G9ScheduleItem.G9ScheduleItem> _saveScheduleTask =
            new SortedDictionary<Guid, G9ScheduleItem.G9ScheduleItem>();

        /// <summary>
        ///     Sorted dictionary for save Schedule items
        /// </summary>
        private static SortedDictionary<Guid, G9ScheduleItem.G9ScheduleItem> SaveScheduleTask
        {
            get
            {
                lock (_saveScheduleTask)
                {
                    // if is null set new instance
                    if (_saveScheduleTask == null)
                        _saveScheduleTask = new SortedDictionary<Guid, G9ScheduleItem.G9ScheduleItem>();
                    // return for use
                    return _saveScheduleTask;
                }
            }
        }

        /// <summary>
        ///     Specify enable Schedule count
        /// </summary>
        public static int ScheduleItemCount => SaveScheduleTask.Count;

        /// <summary>
        ///     Save schedule for this class object
        /// </summary>
        private G9ScheduleItem.G9ScheduleItem _ScheduleItem;

        /// <summary>
        ///     Unique identity for schedule
        /// </summary>
        public Guid ScheduleIdentity { private set; get; }

        /// <summary>
        ///     Save remove items identity
        /// </summary>
        private static readonly Queue<Guid> _removeItemsIdentity = new Queue<Guid>();

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
        public int NumberOfExecution => _ScheduleItem.PeriodCounter;

        #endregion

        #region Methods

        /// <summary>
        ///     Initialize Requirement
        ///     Set timer
        /// </summary>
        /// <param name="addNewSchedule">Specify add new Schedule</param>

        #region Initialize

        private void Initialize(bool addNewSchedule)
        {
            if (addNewSchedule)
            {
                // Set Identity
                _ScheduleItem = new G9ScheduleItem.G9ScheduleItem();
                _ScheduleItem.Identity = ScheduleIdentity = Guid.NewGuid();

                // Add schedule
                SaveScheduleTask.Add(ScheduleIdentity, _ScheduleItem);
            }

            if (!_activeSchedule)
            {
                _activeSchedule = true;
                Task.Run(async () =>
                {
                    while (true)
                        // Check Schedule handler method finished or no
                        if (_waitForFinishScheduleHandler)
                        {
                            // if not finished Delay and check again
                            await Task.Delay(1);
                        }
                        else
                        {
                            // if finished run Schedule handler again and wait
                            await ScheduleHandler();
                            await Task.Delay(1);
                        }
                });
            }
        }

        #endregion

        /// <summary>
        ///     Constructor
        ///     Initialize Requirement
        /// </summary>

        #region G9Scheduled

        public G9Schedule()
        {
            Initialize(true);
        }

        #endregion

        /// <summary>
        ///     Constructor - Restore Schedule by identity
        ///     Initialize Requirement
        /// </summary>
        /// <param name="ScheduleIdentity">Specify Schedule identity for restore enable Schedule</param>

        #region G9Scheduled

        public G9Schedule(Guid ScheduleIdentity)
        {
            if (!SaveScheduleTask.ContainsKey(ScheduleIdentity))
                throw new Exception("Schedule not found!");

            _ScheduleItem = SaveScheduleTask[ScheduleIdentity];
            this.ScheduleIdentity = ScheduleIdentity;

            Initialize(false);
        }

        #endregion

        /// <summary>
        ///     Dispose this Schedule
        /// </summary>

        #region Dispose

        public void Dispose()
        {
            if (SaveScheduleTask[ScheduleIdentity].DisposeCallBack == null)
                SaveScheduleTask[ScheduleIdentity].DisposeCallBack = new HashSet<Action>();
            foreach (var action in SaveScheduleTask[ScheduleIdentity].DisposeCallBack) action?.Invoke();
            SaveScheduleTask.Remove(ScheduleIdentity);

            ScheduleIdentity = Guid.Empty;
            _ScheduleItem = null;
        }

        #endregion

        /// <summary>
        ///     Check validation
        /// </summary>

        #region CheckValidation

        private void CheckValidation()
        {
            if (_ScheduleItem == null) throw new Exception("Object Disposed!");
        }

        #endregion

        /// <summary>
        ///     Add action for Schedule
        /// </summary>
        /// <param name="scheduleAction">Action for Schedule</param>
        /// <returns>Return G9Scheduled</returns>

        #region AddScheduleAction

        public G9Schedule AddScheduleAction(Action scheduleAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].ScheduleAction == null)
                SaveScheduleTask[ScheduleIdentity].ScheduleAction = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].ScheduleAction.Add(scheduleAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Remove action for Schedule
        /// </summary>
        /// <param name="scheduleAction">Action for remove</param>
        /// <returns>Return G9Scheduled</returns>

        #region RemoveScheduleAction

        public G9Schedule RemoveScheduleAction(Action scheduleAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].ScheduleAction == null)
                SaveScheduleTask[ScheduleIdentity].ScheduleAction = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].ScheduleAction.Remove(scheduleAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Add finish call back for Schedule
        /// </summary>
        /// <param name="finishCallBackAction">Action for finish call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region AddFinishCallBack

        public G9Schedule AddFinishCallBack(Action finishCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].FinishCallBack == null)
                SaveScheduleTask[ScheduleIdentity].FinishCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].FinishCallBack.Add(finishCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Remove finish call back for Schedule
        /// </summary>
        /// <param name="finishCallBackAction">Action for remove finish call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region RemoveFinishCallBack

        public G9Schedule RemoveFinishCallBack(Action finishCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].FinishCallBack == null)
                SaveScheduleTask[ScheduleIdentity].FinishCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].FinishCallBack.Remove(finishCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Add stop call back for Schedule
        /// </summary>
        /// <param name="stopCallBackAction">Action for stop call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region AddStopCallBack

        public G9Schedule AddStopCallBack(Action stopCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].StopCallBack == null)
                SaveScheduleTask[ScheduleIdentity].StopCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].StopCallBack.Add(stopCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Remove stop call back for Schedule
        /// </summary>
        /// <param name="stopCallBackAction">Action for remove stop call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region RemoveStopCallBack

        public G9Schedule RemoveStopCallBack(Action stopCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].StopCallBack == null)
                SaveScheduleTask[ScheduleIdentity].StopCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].StopCallBack.Remove(stopCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Add resume call back for Schedule
        /// </summary>
        /// <param name="resumeCallBackAction">Action for resume call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region AddResumeCallBack

        public G9Schedule AddResumeCallBack(Action resumeCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].ResumeCallBack == null)
                SaveScheduleTask[ScheduleIdentity].ResumeCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].ResumeCallBack.Add(resumeCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Remove resume call back for Schedule
        /// </summary>
        /// <param name="resumeCallBackAction">Action for remove resume call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region RemoveResumeCallBack

        public G9Schedule RemoveResumeCallBack(Action resumeCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].ResumeCallBack == null)
                SaveScheduleTask[ScheduleIdentity].ResumeCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].ResumeCallBack.Remove(resumeCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Add Error call back for Schedule
        /// </summary>
        /// <param name="errorCallBackAction">Action for error call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region AddErrorCallBack

        public G9Schedule AddErrorCallBack(Action<Exception> errorCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].ErrorCallBack == null)
                SaveScheduleTask[ScheduleIdentity].ErrorCallBack = new HashSet<Action<Exception>>();
            SaveScheduleTask[ScheduleIdentity].ErrorCallBack.Add(errorCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Remove Error call back for Schedule
        /// </summary>
        /// <param name="errorCallBackAction">Action for remove error call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region RemoveErrorCallBack

        public G9Schedule RemoveErrorCallBack(Action<Exception> errorCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].ErrorCallBack == null)
                SaveScheduleTask[ScheduleIdentity].ErrorCallBack = new HashSet<Action<Exception>>();
            SaveScheduleTask[ScheduleIdentity].ErrorCallBack.Remove(errorCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Add dispose call back for Schedule
        /// </summary>
        /// <param name="disposeCallBackAction">Action for dispose call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region AddDisposeCallBack

        public G9Schedule AddDisposeCallBack(Action disposeCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].DisposeCallBack == null)
                SaveScheduleTask[ScheduleIdentity].DisposeCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].DisposeCallBack.Add(disposeCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Remove dispose call back for Schedule
        /// </summary>
        /// <param name="disposeCallBackAction">Action for remove dispose call back</param>
        /// <returns>Return G9Scheduled</returns>

        #region RemoveDisposeCallBack

        public G9Schedule RemoveDisposeCallBack(Action disposeCallBackAction)
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].DisposeCallBack == null)
                SaveScheduleTask[ScheduleIdentity].DisposeCallBack = new HashSet<Action>();
            SaveScheduleTask[ScheduleIdentity].DisposeCallBack.Remove(disposeCallBackAction);
            return this;
        }

        #endregion

        /// <summary>
        ///     Set start date time for Schedule
        ///     Add filter for Schedule => Specify start date time for Schedule
        /// </summary>
        /// <param name="startDateTime">Specify start date time</param>
        /// <returns>Return G9Scheduled</returns>

        #region SetStartDateTime

        public G9Schedule SetStartDateTime(DateTime startDateTime)
        {
            CheckValidation();
            SaveScheduleTask[ScheduleIdentity].StartDateTime = startDateTime;
            return this;
        }

        #endregion

        /// <summary>
        ///     Set start date time for Schedule
        ///     Add filter for Schedule => Specify finish date time for Schedule
        /// </summary>
        /// <param name="finishDateTime">Specify finish date time</param>
        /// <returns>Return G9Scheduled</returns>

        #region SetFinishDateTime

        public G9Schedule SetFinishDateTime(DateTime finishDateTime)
        {
            CheckValidation();
            SaveScheduleTask[ScheduleIdentity].FinishDateTime = finishDateTime;
            return this;
        }

        #endregion

        /// <summary>
        ///     Set start date time for Schedule
        /// </summary>
        /// <param name="duration">Specify duration for run Schedule action task</param>
        /// <returns>Return G9Scheduled</returns>

        #region SetDuration

        public G9Schedule SetDuration(TimeSpan duration)
        {
            CheckValidation();
            SaveScheduleTask[ScheduleIdentity].Duration = duration;
            return this;
        }

        #endregion

        /// <summary>
        ///     Set Schedule period
        ///     Add filter for Schedule => Number of execution for Schedule
        /// </summary>
        /// <param name="period">Specify number of execution for this Schedule</param>
        /// <returns>Return G9Scheduled</returns>

        #region SetPeriod

        public G9Schedule SetPeriod(int period)
        {
            CheckValidation();
            SaveScheduleTask[ScheduleIdentity].Period = period;
            return this;
        }

        #endregion

        /// <summary>
        ///     Handler for Schedule
        ///     run every time and execute schedule action task
        /// </summary>

        #region ScheduleHandler

        private static async Task ScheduleHandler()
        {
            var oTask = new Task(() =>
            {
                _waitForFinishScheduleHandler = true;
                var currentDateTime = DateTime.Now;

                try
                {
                    foreach (var g9ScheduledItem in SaveScheduleTask)
                        try
                        {
                            // If Schedule disable => stop
                            if (!g9ScheduledItem.Value.EnableSchedule)
                                continue;

                            // If Schedule period enable and greater than PeriodCounter => remove from category and continue
                            if (g9ScheduledItem.Value.Period > 0 &&
                                g9ScheduledItem.Value.PeriodCounter >= g9ScheduledItem.Value.Period)
                            {
                                removeScheduleItem(g9ScheduledItem);
                                continue;
                            }

                            // If acction is null continue
                            if (g9ScheduledItem.Value.ScheduleAction == null ||
                                g9ScheduledItem.Value.Duration == Timeout.InfiniteTimeSpan)
                                continue;

                            // Check start time
                            if (g9ScheduledItem.Value.StartDateTime != DateTime.MinValue &&
                                g9ScheduledItem.Value.StartDateTime > currentDateTime)
                                continue;

                            // Check end date time => remove from category and continue if EndTime < DateTime.Now
                            if (g9ScheduledItem.Value.FinishDateTime != DateTime.MinValue &&
                                g9ScheduledItem.Value.FinishDateTime < currentDateTime)
                            {
                                removeScheduleItem(g9ScheduledItem);
                                continue;
                            }

                            // Check duratuin
                            if (currentDateTime - g9ScheduledItem.Value.LastRunDateTime <
                                g9ScheduledItem.Value.Duration)
                                continue;

                            // Run job
                            foreach (var action in g9ScheduledItem.Value.ScheduleAction) action?.Invoke();

                            // plus period counter when run
                            g9ScheduledItem.Value.PeriodCounter++;

                            // Set last run date time
                            g9ScheduledItem.Value.LastRunDateTime = currentDateTime;
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                foreach (var action in g9ScheduledItem.Value.ErrorCallBack) action?.Invoke(ex);
                            }
                            catch
                            {
                                // Ignore
                            }
                        }
                }
                finally
                {
                    _waitForFinishScheduleHandler = false;
                }
            });
            await oTask;
        }

        #endregion

        /// <summary>
        ///     Remove Schedule item from category
        ///     Helper method
        /// </summary>
        /// <param name="g9ScheduledItem">Specify Schedule item</param>

        #region removeScheduleItem

        private static void removeScheduleItem(KeyValuePair<Guid, G9ScheduleItem.G9ScheduleItem> g9ScheduledItem)
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
                    // Remove
                    SaveScheduleTask.Remove(g9ScheduledItem.Key);
                }
                catch
                {
                    // Ignore
                }
            }
        }

        #endregion

        /// <summary>
        ///     Resume Schedule if stop
        /// </summary>

        #region Resume

        public void Resume()
        {
            CheckValidation();
            if (SaveScheduleTask[ScheduleIdentity].EnableSchedule)
                return;
            if (SaveScheduleTask[ScheduleIdentity].ResumeCallBack == null)
                SaveScheduleTask[ScheduleIdentity].ResumeCallBack = new HashSet<Action>();
            foreach (var action in SaveScheduleTask[ScheduleIdentity].ResumeCallBack) action?.Invoke();
            SaveScheduleTask[ScheduleIdentity].EnableSchedule = true;
        }

        #endregion

        /// <summary>
        ///     Stop Schedule
        /// </summary>

        #region Stop

        public void Stop()
        {
            CheckValidation();
            if (!SaveScheduleTask[ScheduleIdentity].EnableSchedule)
                return;
            SaveScheduleTask[ScheduleIdentity].EnableSchedule = false;
            if (SaveScheduleTask[ScheduleIdentity].StopCallBack == null)
                SaveScheduleTask[ScheduleIdentity].StopCallBack = new HashSet<Action>();
            foreach (var action in SaveScheduleTask[ScheduleIdentity].StopCallBack) action?.Invoke();
        }

        #endregion

        /// <summary>
        ///     Reset duration for current Schedule
        /// </summary>

        #region ResetDuration

        public void ResetDuration()
        {
            SaveScheduleTask[ScheduleIdentity].LastRunDateTime = DateTime.Now;
        }

        #endregion

        #endregion
    }
}