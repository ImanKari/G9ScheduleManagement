using System;
using System.Collections.Generic;
using G9ScheduleManagement.Enum;

namespace G9ScheduleManagement.G9ScheduleItem
{
    /// <summary>
    ///     Data type class for scheduler items
    /// </summary>
    public class G9DtScheduler
    {
        #region Methods

        public G9DtScheduler(G9Scheduler scheduler)
        {
            Scheduler = scheduler;
            StartTime = EndTime = CustomPeriodDuration = TimeSpan.Zero;
            StartDateTime = EndDateTime = DateTime.MinValue;
            CountOfRepetitions = 0;
        }

        #endregion

        #region Fields

        /// <summary>
        ///     Field for accessing to scheduler object
        /// </summary>
        public readonly G9Scheduler Scheduler;

        /// <summary>
        ///     A collection for storing functions related to checking custom conditions.
        /// </summary>
        public HashSet<Func<G9Scheduler, bool>> ConditionFunctions = null;

        /// <summary>
        ///     Helper field for counting the number of repetitions.
        /// </summary>
        public int CountOfRepetitionsCounter = 0;

        /// <summary>
        ///     A collection for storing callbacks related to disposing time.
        /// </summary>
        public HashSet<Action<G9Scheduler, G9EDisposeReason>> DisposeCallbacks = null;

        /// <summary>
        ///     A collection for storing callbacks related to error time.
        /// </summary>
        public HashSet<Action<G9Scheduler, Exception>> ErrorCallbacks = null;

        /// <summary>
        ///     A collection for storing callbacks related to finishing time.
        /// </summary>
        public HashSet<Action<G9Scheduler, G9EFinishingReason, string>> FinishCallbacks = null;

        /// <summary>
        ///     A field for storing the last execution date time.
        /// </summary>
        public DateTime LastRunDateTime = DateTime.MinValue;

        /// <summary>
        ///     A collection for storing callbacks related to the pre-execution time.
        /// </summary>
        public HashSet<Action<G9Scheduler>> PreExecutionCallbacks = null;

        /// <summary>
        ///     A collection for storing callbacks related to the main task of the scheduler.
        /// </summary>
        public HashSet<Action<G9Scheduler>> SchedulerActions = null;

        /// <summary>
        ///     A collection for storing callbacks related to the end-execution time.
        /// </summary>
        public HashSet<Action<G9Scheduler>> EndExecutionCallbacks = null;

        /// <summary>
        ///     Specifies the current state of the scheduler.
        /// </summary>
        public G9ESchedulerState SchedulerState = G9ESchedulerState.NoneState;

        /// <summary>
        ///     Specifies whether the scheduler queue is enabled or not.
        ///     <para />
        ///     If it's set "true," it means that each new scheduler execution must wait for the older one to finish.
        ///     <para />
        ///     If it's set as "false," its meaning is each new scheduler is executed without considering the older one.
        /// </summary>
        public bool IsSchedulerQueueEnable = true;

        /// <summary>
        ///     A collection for storing callbacks related to starting time.
        /// </summary>
        public HashSet<Action<G9Scheduler>> StartCallbacks = null;

        /// <summary>
        ///     A collection for storing callbacks related to stopping time.
        /// </summary>
        public HashSet<Action<G9Scheduler>> StopCallbacks = null;

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
        ///     <para />
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

        /// <summary>
        ///     A field for storing the date time of setting repetition condition.
        /// </summary>
        public DateTime CountOfRepetitionsDateTime;

        /// <summary>
        ///     Specifies that the repetition condition must check per day or in total.
        /// </summary>
        public G9ERepetitionConditionType RepetitionConditionType;

        #endregion
    }
}