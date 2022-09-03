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
        ///     Method to add an action for the scheduler that must perform by paying attention to set conditions.
        /// </summary>
        /// <param name="scheduleAction">Specifies a custom action</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddScheduleAction(Action scheduleAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.ScheduleAction, scheduleAction, nameof(scheduleAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="scheduleAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveScheduleAction(Action scheduleAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.ScheduleAction, scheduleAction, nameof(scheduleAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must execute when the scheduler task is finished.
        /// </summary>
        /// <param name="finishCallbackAction">Specifies a custom action</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddFinishCallback(Action finishCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.FinishCallBack, finishCallbackAction, nameof(finishCallbackAction));
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
        public G9Scheduler RemoveFinishCallback(Action finishCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.FinishCallBack, finishCallbackAction, nameof(finishCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must execute when the scheduler is started.
        /// </summary>
        /// <param name="startCallbackAction">Specifies a custom action</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddStartCallback(Action startCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.StartCallBack, startCallbackAction, nameof(startCallbackAction));
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
        public G9Scheduler RemoveStartCallback(Action startCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.StartCallBack, startCallbackAction, nameof(startCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must execute when the scheduler is stopped.
        /// </summary>
        /// <param name="stopCallbackAction">Specifies a custom action</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddStopCallback(Action stopCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.StopCallBack, stopCallbackAction, nameof(stopCallbackAction));
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
        public G9Scheduler RemoveStopCallback(Action stopCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.StopCallBack, stopCallbackAction, nameof(stopCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must execute when the scheduler is resumed.
        /// </summary>
        /// <param name="resumeCallbackAction">Specifies a custom action</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddResumeCallback(Action resumeCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.ResumeCallBack, resumeCallbackAction, nameof(resumeCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to remove an added action for this event.
        /// </summary>
        /// <param name="resumeCallbackAction">Specifies the older specified action for removing.</param>
        /// <exception cref="Exception">
        ///     An exception is thrown if the specified action for removing doesn't exist in the collection
        ///     of former actions that were added.
        /// </exception>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler RemoveResumeCallback(Action resumeCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.ResumeCallBack, resumeCallbackAction, nameof(resumeCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must execute when the scheduler process faces an error (exception).
        /// </summary>
        /// <param name="errorCallbackAction">Specifies a custom action</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddErrorCallback(Action<Exception> errorCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.ErrorCallBack, errorCallbackAction, nameof(errorCallbackAction));
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
        public G9Scheduler RemoveErrorCallback(Action<Exception> errorCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.ErrorCallBack, errorCallbackAction, nameof(errorCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an action for the scheduler that must execute when the scheduler is removed (dispose).
        /// </summary>
        /// <param name="disposeCallbackAction">Specifies a custom action</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddDisposeCallback(Action<G9EDisposeReason> disposeCallbackAction)
        {
            CheckValidation();
            CollectionAdderHelper(ref _scheduler.DisposeCallBack, disposeCallbackAction,
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
        public G9Scheduler RemoveDisposeCallback(Action<G9EDisposeReason> disposeCallbackAction)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.DisposeCallBack, disposeCallbackAction,
                nameof(disposeCallbackAction));
            return this;
        }

        /// <summary>
        ///     Method to add an function for specifying a custom condition for scheduler execution.
        /// </summary>
        /// <param name="customConditionFunc">Specifies a custom function</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler AddCondition(Func<bool> customConditionFunc)
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
        public G9Scheduler RemoveCondition(Func<bool> customConditionFunc)
        {
            CheckValidation();
            CollectionRemoveHelper(ref _scheduler.ConditionFunctions, customConditionFunc, nameof(customConditionFunc));
            return this;
        }

        /// <summary>
        ///     Method to set (or update) a custom date time for starting as a condition.
        /// </summary>
        /// <param name="startDateTime">
        ///     Specifies a custom date time for starting as a condition.
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
        ///     Method to set (or update)a custom date time for ending as a condition.
        /// </summary>
        /// <param name="finishDateTime">
        ///     Specifies a custom date time for ending as a condition.
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
        ///     Method to set (or update)a custom time for starting as a condition.
        ///     <para />
        ///     The specified time is considered per each day independently.
        /// </summary>
        /// <param name="startTime">
        ///     Specifies a custom time for starting as a condition.
        ///     <para />
        ///     The specified time is considered per each day independently.
        ///     <para />
        ///     If it is set to "TimeSpan.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
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
        ///     Method to set (or update)a custom time for ending as a condition.
        ///     <para />
        ///     The specified time is considered per each day independently.
        ///     <para />
        ///     If it is set to "TimeSpan.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
        ///     condition).
        /// </summary>
        /// <param name="endTime">
        ///     Specifies a custom time for ending as a condition.
        ///     <para />
        ///     The specified time is considered per each day independently.
        ///     <para />
        ///     If it is set to "TimeSpan.Zero", its meaning is that it doesn't have a specified time (indeed, it doesn't have a
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
        ///     Method to set (or update)a custom period duration between each execution.
        /// </summary>
        /// <param name="customPeriodDuration">Specifies a custom period duration between each execution.</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetPeriodDurationBetweenExecutions(TimeSpan customPeriodDuration)
        {
            CheckValidation();
            _scheduler.CustomPeriodDuration = customPeriodDuration;
            return this;
        }

        /// <summary>
        ///     Method to set (or update)a custom count of repetitions for total executions.
        /// </summary>
        /// <param name="customCountOfRepetitions">Specifies a custom count of repetitions for total executions.</param>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler SetCountOfRepetitions(int customCountOfRepetitions)
        {
            CheckValidation();
            _scheduler.CountOfRepetitions = customCountOfRepetitions;
            return this;
        }

        /// <summary>
        ///     Method to start or resume the process of the scheduler.
        /// </summary>
        /// <returns>Access to the main object.</returns>
        /// <exception cref="Exception">An exception is thrown If the scheduler doesn't have any added schedule action.</exception>
        public G9Scheduler StartOrResume()
        {
            if (_scheduler.ScheduleAction == null || !_scheduler.ScheduleAction.Any())
                throw new Exception(
                    $"The scheduler for starting needs to have at least one added schedule action. For adding, you can use the method '{nameof(AddScheduleAction)}.'");
            CheckValidation();
            if (_scheduler.SchedulerState != G9ESchedulerState.Paused &&
                _scheduler.SchedulerState != G9ESchedulerState.Initialized)
                throw new Exception("The scheduler had already been started, and now it can't start again.");
            if (SchedulerState == G9ESchedulerState.Initialized)
            {
                SchedulerState = G9ESchedulerState.StartedWithoutExecution;
                if (_scheduler.StartCallBack != null)
                    foreach (var action in _scheduler.StartCallBack)
                        action?.Invoke();
            }
            else
            {
                SchedulerState = G9ESchedulerState.ResumedWithoutExecution;
                if (_scheduler.ResumeCallBack != null)
                    foreach (var action in _scheduler.ResumeCallBack)
                        action?.Invoke();
            }

            return this;
        }

        /// <summary>
        ///     Method to stop the process of the scheduler.
        /// </summary>
        /// <returns>Access to the main object.</returns>
        public G9Scheduler Stop()
        {
            CheckValidation();
            if (_scheduler.SchedulerState == G9ESchedulerState.None ||
                _scheduler.SchedulerState == G9ESchedulerState.Initialized ||
                _scheduler.SchedulerState == G9ESchedulerState.Paused)
                throw new Exception("The scheduler had already been stopped, and now it can't stop again.");
            _scheduler.SchedulerState = G9ESchedulerState.Paused;
            if (_scheduler.StopCallBack != null)
                foreach (var action in _scheduler.StopCallBack)
                    action?.Invoke();
            return this;
        }
    }
}