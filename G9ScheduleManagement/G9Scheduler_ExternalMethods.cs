using System;
using System.Linq;
using G9ScheduleManagement.DataType;
using G9ScheduleManagement.Enum;

namespace G9ScheduleManagement
{
    /// <summary>
    ///     A pretty small library for working with schedulers
    /// </summary>
    public partial class G9Scheduler
    {
        /// <summary>
        ///     Method to add an action for the scheduler that must run by paying attention to set conditions.
        /// </summary>
        /// <param name="schedulerAction">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddSchedulerAction(Action<G9Scheduler> schedulerAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.SchedulerActions, schedulerAction, nameof(schedulerAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="schedulerAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <exception cref="Exception">
        ///     An exception is thrown if the scheduler has just one action. A scheduler can't work without any action.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveSchedulerAction(Action<G9Scheduler> schedulerAction)
        {
            if (_scheduler.SchedulerActions != null && _scheduler.SchedulerActions.Count == 1 &&
                (SchedulerState == G9ESchedulerState.StartedStateWithoutExecution ||
                 SchedulerState == G9ESchedulerState.StartedStateOnPreExecution ||
                 SchedulerState == G9ESchedulerState.StartedStateOnEndExecution))
                throw new Exception("A scheduler can't work without any scheduler action.");
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.SchedulerActions, schedulerAction, nameof(schedulerAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must be called when the scheduler task wants to run.
        ///     <para />
        ///     Pay attention that each round of scheduler execution calls this callback (event).
        /// </summary>
        /// <param name="preExecutionCallbackAction">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddPreExecutionCallback(Action<G9Scheduler> preExecutionCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.PreExecutionCallbacks, preExecutionCallbackAction,
                nameof(preExecutionCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="preExecutionCallbackAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemovePreExecutionCallback(Action<G9Scheduler> preExecutionCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.PreExecutionCallbacks, preExecutionCallbackAction,
                nameof(preExecutionCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must execute when the scheduler task is ended.
        ///     <para />
        ///     Pay attention that each round of scheduler execution calls this callback (event).
        /// </summary>
        /// <param name="endExecutionCallback">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddEndExecutionCallback(Action<G9Scheduler> endExecutionCallback)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.EndExecutionCallbacks, endExecutionCallback,
                nameof(endExecutionCallback));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="endExecutionCallback">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveEndExecutionCallback(Action<G9Scheduler> endExecutionCallback)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.EndExecutionCallbacks, endExecutionCallback,
                nameof(endExecutionCallback));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must be called when the scheduler task is finished.
        /// </summary>
        /// <param name="finishCallbackAction">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        ///     <para />
        ///     The second callback parameter specifies the reason for finishing.
        ///     <para />
        ///     The third callback parameter specifies an explanation text as the reason (indeed, this text is defined by the
        ///     programmer using the method 'Finish()').
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddFinishCallback(Action<G9Scheduler, G9EFinishingReason, string> finishCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.FinishCallbacks, finishCallbackAction, nameof(finishCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="finishCallbackAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveFinishCallback(Action<G9Scheduler, G9EFinishingReason, string> finishCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.FinishCallbacks, finishCallbackAction, nameof(finishCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must be called when the scheduler is started.
        ///     <para />
        ///     Pay attention that the starting process happens once (using the 'Start()' method).
        /// </summary>
        /// <param name="startCallbackAction">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddStartCallback(Action<G9Scheduler> startCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.StartCallbacks, startCallbackAction, nameof(startCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="startCallbackAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveStartCallback(Action<G9Scheduler> startCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.StartCallbacks, startCallbackAction, nameof(startCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must be called when the scheduler is stopped.
        ///     <para />
        ///     Pay attention that the stopping process happens once (using the 'Stop()' method).
        /// </summary>
        /// <param name="stopCallbackAction">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddStopCallback(Action<G9Scheduler> stopCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.StopCallbacks, stopCallbackAction, nameof(stopCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="stopCallbackAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveStopCallback(Action<G9Scheduler> stopCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.StopCallbacks, stopCallbackAction, nameof(stopCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must be called when the scheduler process faces an error
        ///     (exception).
        /// </summary>
        /// <param name="errorCallbackAction">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        ///     <para />
        ///     The second callback parameter specifies an exception consisting of error information.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddErrorCallback(Action<G9Scheduler, Exception> errorCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.ErrorCallbacks, errorCallbackAction, nameof(errorCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="errorCallbackAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveErrorCallback(Action<G9Scheduler, Exception> errorCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.ErrorCallbacks, errorCallbackAction, nameof(errorCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must be called when the scheduler is removed (dispose).
        /// </summary>
        /// <param name="disposeCallbackAction">
        ///     Specifies a custom action
        ///     <para />
        ///     The first callback parameter provides access to the scheduler.
        ///     <para />
        ///     The second callback parameter specifies the reason for disposing of the scheduler.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddDisposeCallback(Action<G9Scheduler, G9EDisposeReason> disposeCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.DisposeCallbacks, disposeCallbackAction,
                nameof(disposeCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="disposeCallbackAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveDisposeCallback(Action<G9Scheduler, G9EDisposeReason> disposeCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.DisposeCallbacks, disposeCallbackAction,
                nameof(disposeCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add a function for specifying a custom condition for scheduler execution.
        /// </summary>
        /// <param name="customConditionFunc">
        ///     Specifies a custom function
        ///     <para />
        ///     The first function parameter provides access to the scheduler.
        ///     <para />
        ///     The second function parameter specifies the result of the function that must be Boolean.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddCondition(Func<G9Scheduler, bool> customConditionFunc)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.ConditionFunctions, customConditionFunc, nameof(customConditionFunc));
            return this;
        }

        /// <summary>
        ///     Method to remove an added function
        /// </summary>
        /// <param name="customConditionFunc">Specifies the older specified function for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified function for removing doesn't exist in the collection
        ///     of former functions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveCondition(Func<G9Scheduler, bool> customConditionFunc)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.ConditionFunctions, customConditionFunc, nameof(customConditionFunc));
            return this;
        }

        /// <summary>
        ///     Method to set (or update) a Custom Date time for starting as a condition.
        /// </summary>
        /// <param name="startDateTime">
        ///     Specifies a Custom Date time for starting as a condition.
        ///     <para />
        ///     If it is set to "DateTime.MinValue", its meaning is that it doesn't have a start date time (indeed, it doesn't have
        ///     a condition).
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetStartDateTime(DateTime startDateTime)
        {
            CheckValidation();
            _scheduler.StartDateTime = startDateTime;
            return this;
        }

        /// <summary>
        ///     Method to set (or update) a Custom Date time for ending as a condition.
        /// </summary>
        /// <param name="finishDateTime">
        ///     Specifies a Custom Date time for ending as a condition.
        ///     <para />
        ///     If it is set to "DateTime.MinValue", its meaning is that it doesn't have a start date time (indeed, it doesn't have
        ///     a condition).
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetEndDateTime(DateTime finishDateTime)
        {
            CheckValidation();
            _scheduler.EndDateTime = finishDateTime;
            return this;
        }

        /// <summary>
        ///     Method to set (or update) a custom time for starting as a condition.
        ///     <para />
        ///     The specified time is considered for each day independently.
        /// </summary>
        /// <param name="startTime">
        ///     Specifies a custom time for starting as a condition.
        ///     <para />
        ///     The specified time is considered for each day independently.
        ///     <para />
        ///     If it is set to "G9DtTime.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
        ///     condition).
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetStartTime(G9DtTime startTime)
        {
            CheckValidation();
            _scheduler.StartTime = startTime.ConvertToTimeSpan();
            return this;
        }

        /// <summary>
        ///     Method to set (or update) a custom time for ending as a condition.
        ///     <para />
        ///     The specified time is considered for each day independently.
        ///     <para />
        ///     If it is set to "G9DtTime.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
        ///     condition).
        /// </summary>
        /// <param name="endTime">
        ///     Specifies a custom time for ending as a condition.
        ///     <para />
        ///     The specified time is considered for each day independently.
        ///     <para />
        ///     If it is set to "G9DtTime.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
        ///     condition).
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetEndTime(G9DtTime endTime)
        {
            CheckValidation();
            _scheduler.EndTime = endTime.ConvertToTimeSpan();
            return this;
        }

        /// <summary>
        ///     Method to set (or update) a duration period between each execution.
        /// </summary>
        /// <param name="customPeriodDuration">Specifies a custom period duration between each execution.</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetDurationPeriodBetweenExecutions(G9DtGap customPeriodDuration)
        {
            CheckValidation();
            _scheduler.CustomPeriodDuration = customPeriodDuration.ConvertToTimeSpan();
            return this;
        }

        /// <summary>
        ///     Method to set (or update) the count of repetitions for executions.
        /// </summary>
        /// <param name="customCountOfRepetitions">
        ///     Specifies a custom count of repetitions for total executions.
        ///     <para />
        ///     If it's set to 0, its meaning is that it doesn't have a limitation for repetition (indeed, it's infinite).
        /// </param>
        /// <param name="repetitionCondition">
        ///     The second parameter specifies whether the repetition condition must be checked daily.
        ///     <para />
        ///     The repetition condition can finish the scheduler when the repetition type isn't
        ///     <see cref="G9ERepetitionConditionType.PerDay" />.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetCountOfRepetitions(uint customCountOfRepetitions,
            G9ERepetitionConditionType repetitionCondition)
        {
            CheckValidation();
            _scheduler.CountOfRepetitions = customCountOfRepetitions;
            _scheduler.RepetitionsDateTime = DateTime.Now;
            _scheduler.RepetitionConditionType = repetitionCondition;
            return this;
        }

        /// <summary>
        ///     Method to set (or update) the count of tries for unsuccessful execution.
        /// </summary>
        /// <param name="customCountOfTries">
        ///     Specifies a custom count of tries.
        ///     <para />
        ///     If it's set to 0, its meaning is that it doesn't have any tries (for execution again) when an unsuccessful
        ///     execution happens.
        /// </param>
        /// <param name="gapBetweenEachTry">Specifies how much gap there must be between each try.</param>
        /// <returns>Access to the main object.</returns>
        /// <exception cref="Exception">
        ///     The count of tries can't be set before or without specifying the count of repetitions.
        ///     Because trying without repetition limit doesn't work.
        /// </exception>
        public G9Scheduler SetCountOfTries(uint customCountOfTries, G9DtGap gapBetweenEachTry = default)
        {
            CheckValidation();

            if (_scheduler.CountOfRepetitions == 0)
                throw new Exception(
                    $"The count of tries can't be set before or without specifying the count of repetitions (With method '{nameof(SetCountOfRepetitions)}'). Because trying without repetition limit doesn't work.");

            _scheduler.CountOfTries = customCountOfTries;
            _scheduler.GapBetweenEachTry = gapBetweenEachTry.ConvertToTimeSpan();
            return this;
        }

        /// <summary>
        ///     Method to specify the mode of the queue for the scheduler.
        /// </summary>
        /// <param name="isSchedulerQueueEnable">
        ///     Specifies whether the scheduler queue is enabled or not.
        ///     <para />
        ///     If it's set as "true," it means that each new scheduler execution must wait for the older one to finish.
        ///     <para />
        ///     If it's set as "false," its meaning is each new scheduler execution is run without considering the older one.
        ///     <para />
        ///     By default, it's set as "true."
        /// </param>
        /// <returns></returns>
        public G9Scheduler SetQueueMode(bool isSchedulerQueueEnable)
        {
            _scheduler.IsSchedulerQueueEnable = isSchedulerQueueEnable;
            return this;
        }

        /// <summary>
        ///     Method to start or resume the process of the scheduler.
        /// </summary>
        /// <returns>Access to the main object.</returns>
        /// <exception cref="Exception">An exception is thrown If the scheduler doesn't have any added schedule action.</exception>
        public G9Scheduler Start()
        {
            if (_scheduler.SchedulerActions == null || !_scheduler.SchedulerActions.Any())
                throw new Exception(
                    $"The scheduler for starting needs to have at least one added schedule action. For adding, you can use the method '{nameof(AddSchedulerAction)}.'");
            CheckValidation();
            if (_scheduler.SchedulerState != G9ESchedulerState.PausedState &&
                _scheduler.SchedulerState != G9ESchedulerState.InitializedState)
                throw new Exception("The scheduler had already been started, and now it can't start again.");

            SchedulerState = G9ESchedulerState.StartedStateWithoutExecution;
            if (_scheduler.StartCallbacks != null)
                foreach (var action in _scheduler.StartCallbacks)
                    action?.Invoke(this);

            return this;
        }

        /// <summary>
        ///     Method to stop the process of the scheduler.
        /// </summary>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler Stop()
        {
            CheckValidation();
            if (_scheduler.SchedulerState == G9ESchedulerState.NoneState ||
                _scheduler.SchedulerState == G9ESchedulerState.InitializedState ||
                _scheduler.SchedulerState == G9ESchedulerState.PausedState)
                throw new Exception("The scheduler had already been stopped, and now it can't stop again.");
            _scheduler.SchedulerState = G9ESchedulerState.PausedState;
            if (_scheduler.StopCallbacks != null)
                foreach (var action in _scheduler.StopCallbacks)
                    action?.Invoke(this);
            return this;
        }

        /// <summary>
        ///     Method to end the scheduler process.
        ///     <para />
        ///     When a scheduler is finished, it's stopped, the state of that is set on finished, and the finishing callbacks are
        ///     called.
        /// </summary>
        /// <param name="reason">
        ///     Specifies an explanation text for the finish reason.
        ///     <para />
        ///     The specified text passes to the related callback.
        /// </param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler Finish(string reason)
        {
            FinishingHelper(_scheduler, G9EFinishingReason.FinishedByCustomRequest, reason);
            return this;
        }
    }
}