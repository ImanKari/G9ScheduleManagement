using System;

namespace G9ScheduleManagement.DataType
{
    /// <summary>
    ///     Data type for specifying a day time
    /// </summary>
    public readonly struct G9DtTime
    {
        /// <summary>
        ///     Specifies the hour
        /// </summary>
        public readonly byte Hour;

        /// <summary>
        ///     Specifies the minute.
        /// </summary>
        public readonly byte Minute;

        /// <summary>
        ///     Specifies the second
        /// </summary>
        public readonly byte Second;

        /// <summary>
        /// </summary>
        /// <param name="hour">Specifies the hour</param>
        /// <param name="minute">Specifies the minute.</param>
        /// <param name="second">Specifies the second</param>
        public G9DtTime(byte hour, byte minute, byte second)
        {
            if (hour > 23)
                throw new ArgumentException($"The parameter '{nameof(hour)}' can't be greater than 23.", nameof(hour));
            if (minute > 59)
                throw new ArgumentException($"The parameter '{nameof(minute)}' can't be greater than 59.",
                    nameof(minute));
            if (second > 59)
                throw new ArgumentException($"The parameter '{nameof(second)}' can't be greater than 59.",
                    nameof(second));

            Hour = hour;
            Minute = minute;
            Second = second;
        }

        /// <summary>
        ///     Method to convert this data type to TimeSpan.
        /// </summary>
        /// <returns>Converted TimeSpan</returns>
        public TimeSpan ConvertToTimeSpan()
        {
            return new TimeSpan(Hour, Minute, Second);
        }

        /// <summary>
        ///     Method to parse a TimeSpan to time data type.
        /// </summary>
        /// <returns>Converted TimeSpan</returns>
        public static G9DtTime ParseTimeSpan(TimeSpan time)
        {
            return new G9DtTime((byte)time.Hours, (byte)time.Minutes, (byte)time.Seconds);
        }
    }
}