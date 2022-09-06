namespace G9ScheduleManagement.Enum
{
    /// <summary>
    ///     Enum to specify the reasons for disposing of the object.
    /// </summary>
    public enum G9EDisposeReason : byte
    {
        /// <summary>
        ///     It occurs when in a custom process, a scheduler finishes by the 'dispose()' method.
        ///     <para />
        ///     It can happen in a custom programming process by running the 'dispose()' method or automatically when the lifetime
        ///     of
        ///     the scheduler object is finished. The meaning of that is the scheduler stops before its task is done!
        /// </summary>
        DisposedByMethod,

        /// <summary>
        ///     It occurs when the application uses a scheduler and is stopped for any reason.
        /// </summary>
        DisposedOnApplicationStop
    }
}