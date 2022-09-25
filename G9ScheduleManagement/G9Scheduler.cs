using System;
using System.Collections.Generic;
using System.Timers;
using G9ScheduleManagement.Enum;
using G9ScheduleManagement.G9ScheduleItem;
#if !NET35 && !NET40
#endif

namespace G9ScheduleManagement
{
    /// <summary>
    ///     A pretty small library for working with schedulers
    /// </summary>
    [Serializable]
    public partial class G9Scheduler
    {
        /// <summary>
        ///     An object for managing lock the multi-thread access process.
        /// </summary>
        private static readonly object LockCollectionForScheduleTask = new object();

        /// <summary>
        ///     A category for storing run-time schedulers
        /// </summary>
        private static readonly Dictionary<uint, G9DtScheduler> SchedulersCollection =
            new Dictionary<uint, G9DtScheduler>();

        /// <summary>
        ///     Cancellation token
        /// </summary>
        private static bool _cancelToken;

        /// <summary>
        ///     It's a flag that at the start of each round is set to 'false,' and at the end of each round of execution is set to
        ///     'true' again.
        ///     <para />
        ///     Indeed, it makes sure that all schedulers in each round are executed.
        /// </summary>
        private static bool _isSchedulerRoundIsFinished = true;

        /// <summary>
        ///     A counter for specifying the unique identity of the scheduler
        /// </summary>
        private static uint _scheduleIdentityIndex = 1;

        /// <summary>
        ///     A static field for storing the main timer of all schedulers.
        /// </summary>
        private static Timer _mainTimer;

        /// <summary>
        ///     Specifies the current scheduler item for the created object.
        /// </summary>
        private G9DtScheduler _scheduler;

        /// <summary>
        ///     Specifies the count of current schedulers.
        /// </summary>
        public static int SchedulerItemCount
        {
            get
            {
                lock (LockCollectionForScheduleTask)
                {
                    return SchedulersCollection.Count;
                }
            }
        }

        /// <summary>
        ///     Specifies the unique identity of the scheduler.
        /// </summary>
        public uint ScheduleIdentity { private set; get; }

        /// <summary>
        ///     Specifies the current state of the scheduler.
        /// </summary>
        public G9ESchedulerState SchedulerState
        {
            get => _scheduler.SchedulerState;
            private set => _scheduler.SchedulerState = value;
        }
    }
}