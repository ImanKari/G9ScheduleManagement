using System;

namespace G9ScheduleManagement.DataType
{
    /// <summary>
    ///     Gap type for specifying a gap
    /// </summary>
    public readonly struct G9DtGap
    {
        /// <summary>
        ///     Specifies the days
        /// </summary>
        public readonly ushort Days;

        /// <summary>
        ///     Specifies the hours
        /// </summary>
        public readonly ushort Hours;

        /// <summary>
        ///     Specifies the minutes.
        /// </summary>
        public readonly ushort Minutes;

        /// <summary>
        ///     Specifies the seconds
        /// </summary>
        public readonly ushort Seconds;

        /// <summary>
        ///     Specifies the milliseconds
        /// </summary>
        public readonly ushort Milliseconds;

        /// <summary>
        ///     Specifies zero time
        /// </summary>
        public static readonly G9DtGap Zero = new G9DtGap(0, 0, 0, 0, 0);

        /// <summary>
        ///     Specifies one second
        /// </summary>
        public static readonly G9DtGap OneSec = new G9DtGap(0, 0, 0, 1, 0);

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="days">Specifies the days</param>
        /// <param name="hours">Specifies the hours</param>
        /// <param name="minutes">Specifies the minutes</param>
        /// <param name="seconds">Specifies the seconds</param>
        /// <param name="milliseconds">Specifies the milliseconds</param>
        private G9DtGap(ushort days, ushort hours, ushort minutes, ushort seconds, ushort milliseconds)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Days = days;
            Milliseconds = milliseconds;
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="days">Specifies the days</param>
        /// <param name="hours">Specifies the hours</param>
        /// <param name="minutes">Specifies the minutes</param>
        /// <param name="seconds">Specifies the seconds</param>
        /// <param name="milliseconds">Specifies the milliseconds</param>
        public static G9DtGap Init(ushort days, ushort hours, ushort minutes, ushort seconds, ushort milliseconds)
        {
            return new G9DtGap(days, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="days">Specifies the days</param>
        public static G9DtGap FromDays(ushort days)
        {
            return new G9DtGap(days, 0, 0, 0, 0);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="hours">Specifies the hours</param>
        public static G9DtGap FromHours(ushort hours)
        {
            return new G9DtGap(0, hours, 0, 0, 0);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="minutes">Specifies the minutes</param>
        public static G9DtGap FromMinutes(ushort minutes)
        {
            return new G9DtGap(0, 0, minutes, 0, 0);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="seconds">Specifies the seconds</param>
        public static G9DtGap FromSeconds(ushort seconds)
        {
            return new G9DtGap(0, 0, 0, seconds, 0);
        }

        /// <summary>
        ///     Initializer
        /// </summary>
        /// <param name="milliseconds">Specifies the milliseconds</param>
        public static G9DtGap FromMilliseconds(ushort milliseconds)
        {
            return new G9DtGap(0, 0, 0, 0, milliseconds);
        }

        /// <summary>
        ///     Method to convert this data type to TimeSpan.
        /// </summary>
        /// <returns>Converted TimeSpan</returns>
        public TimeSpan ConvertToTimeSpan()
        {
            return new TimeSpan(Days, Hours, Minutes, Seconds, Milliseconds);
        }

        /// <summary>
        ///     Method to parse a TimeSpan to time data type.
        /// </summary>
        /// <returns>Converted TimeSpan</returns>
        public static G9DtGap ParseTimeSpan(TimeSpan time)
        {
            return new G9DtGap((ushort)time.Days, (ushort)time.Hours, (ushort)time.Minutes, (ushort)time.Seconds,
                (ushort)time.Milliseconds);
        }
    }
}