using System;
using System.Collections.Generic;
using System.Threading;

namespace G9ScheduleManagement.G9ScheduleItem
{
    /// <summary>
    ///     Class data type for Schedule item
    /// </summary>
    public class G9ScheduleItem
    {
        /// <summary>
        ///     Category for save dispose call back
        /// </summary>
        public HashSet<Action> DisposeCallBack = null;

        /// <summary>
        ///     Field save duration between run Schedule
        /// </summary>
        public TimeSpan Duration = Timeout.InfiniteTimeSpan;

        /// <summary>
        ///     Specify Schedule is enable or no
        ///     if stop Schedule disable this field
        /// </summary>
        public bool EnableSchedule = true;

        /// <summary>
        ///     Category for save error call back
        /// </summary>
        public HashSet<Action<Exception>> ErrorCallBack = null;

        /// <summary>
        ///     Category for save finish call back
        /// </summary>
        public HashSet<Action> FinishCallBack = null;

        /// <summary>
        ///     Field for save finish date time
        ///     if set "DateTime.MinValue" => Infinity
        /// </summary>
        public DateTime FinishDateTime = DateTime.MinValue;

        /// <summary>
        ///     Specify Identity of Schedule
        /// </summary>
        public Guid Identity;

        /// <summary>
        ///     Field save last run Schedule for calculate duration
        /// </summary>
        public DateTime LastRunDateTime = DateTime.MinValue;

        /// <summary>
        ///     Specify Schedule period
        ///     if set lower our equal than 0 => Infinity
        /// </summary>
        public int Period = 0;

        /// <summary>
        ///     field for save Schedule period counter
        /// </summary>
        public int PeriodCounter = 0;

        /// <summary>
        ///     Category for resume call back
        /// </summary>
        public HashSet<Action> ResumeCallBack = null;

        /// <summary>
        ///     Category for save Schedule action
        /// </summary>
        public HashSet<Action> ScheduleAction = null;

        /// <summary>
        ///     Field save starter date time for Schedule
        ///     if set "DateTime.MinValue" => Infinity
        /// </summary>
        public DateTime StartDateTime = DateTime.MinValue;

        /// <summary>
        ///     Category for save stop call back
        /// </summary>
        public HashSet<Action> StopCallBack = null;
    }
}