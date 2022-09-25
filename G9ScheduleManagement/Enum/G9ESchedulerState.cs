namespace G9ScheduleManagement.Enum
{
    /// <summary>
    ///     An enum for specifying the current state of a scheduler
    /// </summary>
    public enum G9ESchedulerState : byte
    {
        /// <summary>
        ///     None
        /// </summary>
        NoneState,

        /// <summary>
        ///     Specifies that the scheduler just initialized
        /// </summary>
        InitializedState,

        /// <summary>
        ///     Specifies that the scheduler was Started (or resumed).
        ///     <para />
        ///     This state happens when the scheduler has been started (or resumed), but according to specified conditions, it
        ///     hasn't had
        ///     execution.
        /// </summary>
        StartedStateWithoutExecution,

        /// <summary>
        ///     Specifies that the scheduler was Started (or resumed).
        ///     <para />
        ///     This state happens when the scheduler has started (or resumed) and has had at least one current unfinished
        ///     execution.
        /// </summary>
        StartedStateOnPreExecution,

        /// <summary>
        ///     Specifies that the scheduler was Started (or resumed).
        ///     <para />
        ///     This state happens when the scheduler has started (or resumed) and has had at least one current finished execution.
        /// </summary>
        StartedStateOnEndExecution,

        /// <summary>
        ///     Specifies that the scheduler was paused.
        /// </summary>
        PausedState,

        /// <summary>
        ///     Specifies that the scheduler has had at least one error (Exception).
        /// </summary>
        HasError,

        /// <summary>
        ///     Specifies that the scheduler task was finished.
        /// </summary>
        FinishedState,

        /// <summary>
        ///     Specifies that scheduler execution rejected according to set conditions.
        /// </summary>
        ConditionalRejectExecution
    }
}