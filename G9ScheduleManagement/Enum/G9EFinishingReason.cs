namespace G9ScheduleManagement.Enum
{
    /// <summary>
    ///     Enum for specifying the reason for finishing
    /// </summary>
    public enum G9EFinishingReason : byte
    {
        /// <summary>
        ///     Specifies that the condition of the count of repetitions is finished.
        ///     <para />
        ///     The repetition condition can finish the scheduler when the repetition type isn't
        ///     <see cref="G9ERepetitionConditionType.PerDay" />.
        /// </summary>
        FinishedByRepetitionCondition,

        /// <summary>
        ///     Specifies that the end date time condition caused to finish of the process.
        /// </summary>
        FinishedByEndDateTimeCondition,

        /// <summary>
        ///     Specifies that the scheduler was finished by a custom request (its meaning is that the method 'A' was called).
        /// </summary>
        FinishedByCustomRequest
    }
}