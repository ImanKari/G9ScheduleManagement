using System;
using System.Collections.Generic;
using G9ScheduleManagement.Enum;
#if !NET35 && !NET40
using System.Threading;
#endif

namespace G9ScheduleManagement.G9ScheduleItem
{
    /// <summary>
    ///     Data type class for scheduler items
    /// </summary>
    public class G9DtScheduler
    {
        #region Methods

        public G9DtScheduler()
        {
            StartTime = EndTime = CustomPeriodDuration = TimeSpan.Zero;
            StartDateTime = EndDateTime = DateTime.MinValue;
            CountOfRepetitions = 0;
        }

        #endregion

        #region Fields

        /// <summary>
        ///     A collection for storing functions related to checking custom conditions.
        /// </summary>
        public HashSet<Func<bool>> ConditionFunctions = null;

        /// <summary>
        ///     Helper field for counting the number of repetitions.
        /// </summary>
        public int CountOfRepetitionsCounter = 0;

        /// <summary>
        ///     A collection for storing callbacks related to disposing time.
        /// </summary>
        public HashSet<Action<G9EDisposeReason>> DisposeCallBack = null;

        /// <summary>
        ///     A collection for storing callbacks related to error time.
        /// </summary>
        public HashSet<Action<Exception>> ErrorCallBack = null;

        /// <summary>
        ///     A collection for storing callbacks related to finishing time.
        /// </summary>
        public HashSet<Action> FinishCallBack = null;

        /// <summary>
        ///     A field for storing the last execution date time.
        /// </summary>
        public DateTime LastRunDateTime = DateTime.Now;

        /// <summary>
        ///     A collection for storing callbacks related to resuming time.
        /// </summary>
        public HashSet<Action> ResumeCallBack = null;

        /// <summary>
        ///     A collection for storing callbacks related to the main task of the scheduler.
        /// </summary>
        public HashSet<Action> ScheduleAction = null;

        /// <summary>
        ///     Specifies the current state of the scheduler.
        /// </summary>
        public G9ESchedulerState SchedulerState = G9ESchedulerState.None;

        /// <summary>
        ///     A collection for storing callbacks related to starting time.
        /// </summary>
        public HashSet<Action> StartCallBack = null;

        /// <summary>
        ///     A collection for storing callbacks related to stopping time.
        /// </summary>
        public HashSet<Action> StopCallBack = null;

        #endregion

        #region Peroperties

        /// <summary>
        ///     Private field
        /// </summary>
        private DateTime _endDateTime;


        /// <summary>
        ///     Specifies the end date time for scheduler.
        ///     <para />
        ///     If it is set to "DateTime.MinValue", its meaning is that it doesn't have a start date time (indeed, it doesn't have
        ///     a condition).
        /// </summary>
        public DateTime EndDateTime
        {
            set
            {
                HasEndDateTime = value != DateTime.MinValue;
                _endDateTime = value;
            }
            get => _endDateTime;
        }

        /// <summary>
        ///     Specifies that scheduler has a end date time condition or not.
        /// </summary>
        public bool HasEndDateTime { private set; get; }

        /// <summary>
        ///     Private field
        /// </summary>
        private DateTime _startDateTime;

        /// <summary>
        ///     Specifies the start date time for scheduler.
        ///     <para />
        ///     If it is set to "DateTime.MinValue", its meaning is that it doesn't have a start date time (indeed, it doesn't have
        ///     a condition).
        /// </summary>
        public DateTime StartDateTime
        {
            set
            {
                HasStartDateTime = value != DateTime.MinValue;
                _startDateTime = value;
            }
            get => _startDateTime;
        }

        /// <summary>
        ///     Specifies that scheduler has a start date time condition or not.
        /// </summary>
        public bool HasStartDateTime { private set; get; }

        /// <summary>
        ///     Private field
        /// </summary>
        private TimeSpan _startTime;

        /// <summary>
        ///     Specifies the start time for the scheduler for each day.
        ///     <para />
        ///     If it is set to "TimeSpan.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
        ///     condition).
        /// </summary>
        public TimeSpan StartTime
        {
            set
            {
                HasStartTime = value != TimeSpan.Zero;
                _startTime = value;
            }
            get => _startTime;
        }

        /// <summary>
        ///     Specifies that scheduler has a start time condition or not.
        /// </summary>
        public bool HasStartTime { private set; get; }

        /// <summary>
        ///     Private field
        /// </summary>
        private TimeSpan _endTime;

        /// <summary>
        ///     Specifies the end time for the scheduler for each day.
        ///     <para />
        ///     If it is set to "TimeSpan.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
        ///     condition).
        /// </summary>
        public TimeSpan EndTime
        {
            set
            {
                HasEndTime = value != TimeSpan.Zero;
                _endTime = value;
            }
            get => _endTime;
        }

        /// <summary>
        ///     Specifies that scheduler has a end time condition or not.
        /// </summary>
        public bool HasEndTime { private set; get; }

        /// <summary>
        ///     Private field
        /// </summary>
        private TimeSpan _customPeriodDuration;

        /// <summary>
        ///     Specifies a custom period duration between each execution.
        ///     <para />
        ///     If it is set to "TimeSpan.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
        ///     condition).
        /// </summary>
        public TimeSpan CustomPeriodDuration
        {
            set
            {
                HasCustomPeriodDuration = value != TimeSpan.Zero;
                _customPeriodDuration = value;
            }
            get => _customPeriodDuration;
        }

        /// <summary>
        ///     Specifies that a custom period duration between each execution is set or not
        /// </summary>
        public bool HasCustomPeriodDuration { private set; get; }

        /// <summary>
        ///     Private field
        /// </summary>
        private int _countOfRepetitions;

        /// <summary>
        ///     Specifies the number of repetitions for scheduled action.
        ///     If it's set to 0, its meaning is that it doesn't have a limitation for repetition (indeed, it's infinite).
        /// </summary>
        public int CountOfRepetitions
        {
            set
            {
                HasCustomCountOfRepetitions = value != 0;
                _countOfRepetitions = value;
            }
            get => _countOfRepetitions;
        }

        /// <summary>
        ///     Specifies that a custom period duration between each execution is set or not
        /// </summary>
        public bool HasCustomCountOfRepetitions { private set; get; }

        #endregion
    }
}