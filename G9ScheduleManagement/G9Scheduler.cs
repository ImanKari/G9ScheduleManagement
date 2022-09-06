using System;
using System.Collections.Generic;
using System.Threading;
using G9ScheduleManagement.Enum;
using G9ScheduleManagement.G9ScheduleItem;
#if !NET35 && !NET40
using System.Threading.Tasks;
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
        ///     Specifies the count of current schedulers.
        /// </summary>
        public static int SchedulerItemCount
        {
            get
            {
#if NET35
                lock (LockCollectionForScheduleTask)
                {
                    return SchedulersCollection.Count;
                }
#else
                return SchedulersCollection.Count;
#endif
            }
        }

        /// <summary>
        ///     Cancellation token
        /// </summary>
#if NET35 || NET40
        private static bool _cancelToken = false;
#else
        private static readonly CancellationTokenSource CancelToken = new CancellationTokenSource();
#endif

        /// <summary>
        ///     Specifies the current scheduler item for the created object.
        /// </summary>
        private G9DtScheduler _scheduler;

        /// <summary>
        ///     A counter for specifying the unique identity of the scheduler
        /// </summary>
        private static uint _scheduleIdentityIndex = 1;

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

        /// <summary>
        ///     A static field for storing the main thread of scheduler management.
        /// </summary>
#if NET35 || NET40
        private static Thread _mainTask;
#else
        private static Task _mainTask;
#endif
    }
}