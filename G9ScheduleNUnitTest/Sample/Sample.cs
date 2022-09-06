using System;
using G9ScheduleManagement;
using G9ScheduleManagement.DataType;
using G9ScheduleManagement.Enum;

public class Sample
{
    public void SampleScheduler()
    {
        var scheduler = new G9Scheduler()
            // ################################### Callbacks ###################################

            // Method to add an action for the scheduler that must run by paying attention to set conditions.
            .AddSchedulerAction(sc => Console.Write(sc.SchedulerState)) //Has a remove method.

            // Called when the scheduler task wants to run.
            // Note: Pay attention that each round of scheduler execution calls this callback (event).
            .AddPreExecutionCallback(sc => Console.Write(sc.SchedulerState)) // Has a remove method.

            // Called when the scheduler task is ended.
            // Note: Pay attention that each round of scheduler execution calls this callback (event).
            .AddEndExecutionCallback(sc => Console.Write(sc.SchedulerState)) // Has a remove method.

            // Called when the scheduler is started.
            // Note: Pay attention that the starting process happens once (using the 'Start()' method).
            .AddStartCallback(sc => Console.Write(sc.SchedulerState)) // Has a remove method.

            // Called when the scheduler is stopped.
            // Note: Pay attention that the stopping process happens once (using the 'Stop()' method).
            .AddStopCallback(sc => Console.Write(sc.SchedulerState)) // Has a remove method.

            // Called when the scheduler task is finished.
            // The second callback parameter specifies the reason for finishing.
            // The third callback parameter specifies a custom text as the reason (indeed, this text is defined by the programmer).
            .AddFinishCallback((sc, reason, text) => Console.Write($"Reason: {reason} | Text: {text}")) // Has a remove method.

            // Called when the scheduler is removed (dispose).
            // The second callback parameter specifies the reason for disposing of the scheduler.
            .AddDisposeCallback((sc, reason) => Console.Write(reason)) // Has a remove method.

            // Called when the scheduler process faces an error (exception).
            // Note: The scheduler doesn't throw any exceptions, so this callback must handle exceptions.
            // The second callback parameter specifies an exception consisting of error information.
            .AddErrorCallback((sc, ex) => Console.WriteLine(ex.Message)) // Has a remove method.

            // ################################### Conditions ###################################

            // Method to add a function for specifying a custom condition for scheduler execution.
            // The second function parameter specifies the result of the function that must be Boolean.
            .AddCondition(sc => DateTime.Now < DateTime.Parse("2026-09-01")) // Has a remove method.

            // Method to set (or update) a custom Date Time for starting/ending as a condition.
            .SetStartDateTime(DateTime.Now)
            .SetEndDateTime(DateTime.Now.AddDays(9))

            // Method to set (or update) the count of repetitions for executions.
            // The second parameter specifies whether the repetition condition must be checked daily.
            .SetCountOfRepetitions(99, G9ERepetitionConditionType.PerDay)

            // Method to set (or update) a duration period between each execution.
            .SetDurationPeriodBetweenExecutions(TimeSpan.FromSeconds(1))

            // Method to set (or update) a custom time for starting/ending as a condition.
            // The specified time is considered for each day independently.
            .SetStartTime(new G9DtTime(10, 0, 0))
            .SetEndTime(new G9DtTime(16, 0, 0))

            // Method to specify the mode of the queue for the scheduler.
            // If it's set as "true," it means that each new scheduler execution must wait for the older one to finish.
            // If it's set as "false," its meaning is each new scheduler execution is run without considering the older one.
            // By default, it's set as "true."
            .SetQueueMode(true);

        // Starting
        scheduler.Start();

        // ################################### Properties ###################################
        // Specifies the current state of the scheduler.
        Console.WriteLine(scheduler.SchedulerState);
        // Specifies the unique identity of the scheduler.
        Console.WriteLine(scheduler.ScheduleIdentity);

        // ################################### Other ################################### ###
        // Method to stop the process of the scheduler.
        scheduler.Stop();
        // Method to end the scheduler process.
        // When a scheduler is finished, it's stopped, the state of that is set on finished, and the finishing callbacks are called.
        scheduler.Finish("The main task of the scheduler is finished.");
        // Method to dispose
        scheduler.Dispose();
    }
}