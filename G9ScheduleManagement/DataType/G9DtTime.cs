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
        ///     Specifies the milliseconds
        /// </summary>
        public readonly ushort Milliseconds;

        /// <summary>
        ///     Specifies zero time
        /// </summary>
        public static readonly G9DtTime Zero = new G9DtTime(0, 0, 0);

        /// <summary>
        ///     Specifies one second
        /// </summary>
        public static readonly G9DtTime OneSec = new G9DtTime(0, 0, 1);

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="hour">Specifies the hour</param>
        /// <param name="minute">Specifies the minute.</param>
        /// <param name="second">Specifies the second</param>
        /// <param name="milliseconds">Specifies the milliseconds</param>
        private G9DtTime(byte hour, byte minute, byte second, ushort milliseconds = 0)
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
            Milliseconds = milliseconds;
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="hour">Specifies the hour</param>
        /// <param name="minute">Specifies the minute.</param>
        /// <param name="second">Specifies the second</param>
        /// <param name="milliseconds">Specifies the milliseconds</param>
        public static G9DtTime Init(byte hour, byte minute, byte second, ushort milliseconds)
        {
            return new G9DtTime(hour, minute, second, milliseconds);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="hours">Specifies the hours</param>
        public static G9DtTime FromHours(byte hours)
        {
            return new G9DtTime(hours, 0, 0);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="minutes">Specifies the minutes</param>
        public static G9DtTime FromMinutes(byte minutes)
        {
            return new G9DtTime(0, minutes, 0);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="seconds">Specifies the seconds</param>
        public static G9DtTime FromSeconds(byte seconds)
        {
            return new G9DtTime(0, 0, seconds);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="milliseconds">Specifies the milliseconds</param>
        public static G9DtTime FromMilliseconds(byte milliseconds)
        {
            return new G9DtTime(0, 0, 0, milliseconds);
        }

        /// <summary>
        ///     Method to convert this data type to TimeSpan.
        /// </summary>
        /// <returns>Converted TimeSpan</returns>
        public TimeSpan ConvertToTimeSpan()
        {
            return new TimeSpan(0, Hour, Minute, Second, Milliseconds);
        }

        /// <summary>
        ///     Method to parse a TimeSpan to time data type.
        /// </summary>
        /// <returns>Converted TimeSpan</returns>
        public static G9DtTime ParseTimeSpan(TimeSpan time)
        {
            return new G9DtTime((byte)time.Hours, (byte)time.Minutes, (byte)time.Seconds, (ushort)time.Milliseconds);
        }
    }
}