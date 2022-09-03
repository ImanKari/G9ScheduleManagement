namespace G9ScheduleManagement.Enum
{
    /// <summary>
    ///     An enum for specifying the current state of a scheduler
    /// </summary>
    public enum G9ESchedulerState
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the scheduler just initialized
        /// </summary>
        Initialized,

        /// <summary>
        ///     Specifies that the scheduler was Started.
        ///     <para />
        ///     This state happens when the scheduler has been started, but according to specified conditions, it hasn't had
        ///     execution.
        /// </summary>
        StartedWithoutExecution,

        /// <summary>
        ///     Specifies that the scheduler was Started.
        ///     <para />
        ///     This state happens when the scheduler has started and has had at least one execution.
        /// </summary>
        StartedWithExecution,

        /// <summary>
        ///     Specifies that the scheduler was paused.
        /// </summary>
        Paused,

        /// <summary>
        ///     Specifies that the scheduler was resumed.
        ///     <para />
        ///     This state happens when the scheduler starts again after a stop.
        ///     <para />
        ///     In addition, specifies that the scheduler has been started, but according to specified conditions, it hasn't had
        ///     execution.
        /// </summary>
        ResumedWithoutExecution,

        /// <summary>
        ///     Specifies that the scheduler was resumed.
        ///     <para />
        ///     This state happens when the scheduler starts again after a stop.
        ///     <para />
        ///     In addition, specifies that the scheduler has started and has had at least one execution.
        /// </summary>
        ResumedWithExecution,

        /// <summary>
        ///     Specifies that the scheduler has had at least one error.
        /// </summary>
        HasError,

        /// <summary>
        ///     Specifies that the scheduler task was finished.
        /// </summary>
        Finished
    }
}