using System;

namespace G9ScheduleManagement
{
    /// <summary>
    ///     A pretty small library for working with schedulers
    /// </summary>
    public partial class G9Scheduler
    {
        /// <summary>
        ///     Method to generate a custom event with a custom condition.
        /// </summary>
        /// <param name="condition">Specifies a custom condition that leads to executing event callback.</param>
        /// <param name="callback">Specifies a custom callback that calls when the specified condition is true.</param>
        /// <param name="durationPeriodOfChecking">
        ///     Specifies a duration period for checking the specified condition.
        ///     <para />
        ///     By default, it's set to 1 second.
        /// </param>
        /// <returns>A scheduler object</returns>
        public static G9Scheduler GenerateCustomEvent(Func<G9Scheduler, bool> condition, Action<G9Scheduler> callback,
            TimeSpan durationPeriodOfChecking = default)
        {
            if (Equals(durationPeriodOfChecking, default(TimeSpan)))
                durationPeriodOfChecking = TimeSpan.FromSeconds(1);

            return new G9Scheduler()
                .AddSchedulerAction(callback)
                .AddCondition(condition)
                .SetDurationPeriodBetweenExecutions(durationPeriodOfChecking);
        }

        /// <summary>
        ///     Method to generate a custom event sensitive to a value change.
        /// </summary>
        /// <param name="source">Specifies a source object for accessing its members.</param>
        /// <param name="selector">
        ///     Specifies a selector for choosing a member.
        ///     <para />
        ///     The value of the chosen member automatically checks by core and if any change occurs. The specified callback is
        ///     executed.
        /// </param>
        /// <param name="callback">
        ///     Specifies a custom callback that calls when the chosen member value has changed.
        ///     <para />
        ///     This callback has two parameters that specify the old value on the first parameter and the new value on the second
        ///     parameter.
        /// </param>
        /// <param name="durationPeriodOfChecking">
        ///     Specifies a duration period for checking the specified condition.
        ///     <para />
        ///     By default, it's set to 1 second.
        /// </param>
        /// <returns>A scheduler object</returns>
        public static G9Scheduler GenerateCustomEventOnValueChange<TSource, TResult>(TSource source,
            Func<TSource, TResult> selector, Action<TResult, TResult> callback,
            TimeSpan durationPeriodOfChecking = default)
        {
            // Storing the first value for checking.
            var oldValue = selector(source);

            // Local function for condition.
            bool CustomCondition(G9Scheduler scheduler)
            {
                return !Equals(oldValue, selector(source));
            }

            // Local function for action0
            void CustomAction(G9Scheduler scheduler)
            {
                var old = oldValue;
                oldValue = selector(source);
                callback(old, oldValue);
            }

            // Create scheduler with custom condition and action
            return GenerateCustomEvent(CustomCondition, CustomAction, durationPeriodOfChecking);
        }
    }
}