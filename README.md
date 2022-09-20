[![G9TM](https://raw.githubusercontent.com/ImanKari/G9ScheduleManagement/master/Asset/G9ScheduleManagement.png)](http://www.g9tm.com/) **G9ScheduleManagement**

[![NuGet version (G9AssemblyManagement)](https://img.shields.io/nuget/v/G9ScheduleManagement.svg?style=flat-square)](https://www.nuget.org/packages/G9ScheduleManagement/)
[![Azure DevOps Pipeline Build Status](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/AzureDevOpsPipelineBuildStatus.png)](https://g9tm.visualstudio.com/G9ScheduleManagement/_apis/build/status/G9ScheduleManagement?branchName=master)
[![Github Repository](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/GitHub.png)](https://github.com/ImanKari/G9ScheduleManagement)


# G9ScheduleManagement
## A lightweight .NET library has been developed for interacting with schedulers. Many valuable tools for developing a custom scheduler are available within this package that allows you to create various types of schedulers with a vast scale of desired Callbacks, Conditions, and Tools.

# Overview
Before anything, we would be familiar with almost all the tools in this package, along with a small example. Then, some functional examples, along with implementation, will be shown.\
Points:
- All methods in this library return the created instance of their object, so you can use another method along with a dot after using the first method.
- All callback methods have removal methods that start with "remove..." like "RemoveSchedulerAction()".
- All condition methods can be set or updated at any time.
```csharp
using System;
using G9ScheduleManagement;
using G9ScheduleManagement.DataType;
using G9ScheduleManagement.Enum;

internal class Program
{
    private static void Main()
    {
        var scheduler = new G9Scheduler()
            // [Callback Methods:]

            // Method to add an action for the scheduler that must run by paying attention to set conditions.
            .AddSchedulerAction(sc => Console.Write(sc.SchedulerState))

            // Called when the scheduler task wants to run.
            // Note: Pay attention that each round of scheduler execution calls this callback (event).
            .AddPreExecutionCallback(sc => Console.Write(sc.SchedulerState)) 

            // Called when the scheduler task is ended.
            // Note: Pay attention that each round of scheduler execution calls this callback (event).
            .AddEndExecutionCallback(sc => Console.Write(sc.SchedulerState)) 

            // Called when the scheduler is started.
            // Note: Pay attention that the starting process happens once (using the 'Start()' method).
            .AddStartCallback(sc => Console.Write(sc.SchedulerState)) 

            // Called when the scheduler is stopped.
            // Note: Pay attention that the stopping process happens once (using the 'Stop()' method).
            .AddStopCallback(sc => Console.Write(sc.SchedulerState))

            // Called when the scheduler task is finished.
            // The second callback parameter specifies the reason for finishing.
            .AddFinishCallback((sc, reason) => Console.Write(reason))

            // Called when the scheduler is removed (dispose).
            // The second callback parameter specifies the reason for disposing of the scheduler.
            .AddDisposeCallback((sc, reason) => Console.Write(reason))

            // Called when the scheduler process faces an error (exception).
            // Note: The scheduler doesn't throw any exceptions, so this callback must handle exceptions.
            // The second callback parameter specifies an exception consisting of error information.
            .AddErrorCallback((sc, ex) => Console.WriteLine(ex.Message))

            // [Condition Methods:]

            // Method to add a function for specifying a custom condition for scheduler execution.
            // The second function parameter specifies the result of the function that must be Boolean.
            // It also has the removal method "RemoveCondition()"
            .AddCondition(sc => DateTime.Now < DateTime.Parse("2026-09-01"))

            // Method to set (or update) a custom Date Time for starting/ending as a condition.
            .SetStartDateTime(DateTime.Now)
            .SetEndDateTime(DateTime.Now.AddDays(9))

            // Method to set (or update) the count of repetitions for executions.
            // The second parameter specifies whether the repetition condition must be checked daily or not.
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

        // [Starting Method:]
        scheduler.Start();

        // [Properties:]
        // Specifies the current state of the scheduler.
        Console.WriteLine(scheduler.SchedulerState);
        // Specifies the unique identity of the scheduler.
        Console.WriteLine(scheduler.ScheduleIdentity);

        // [Other:]
        // Method to stop the process of the scheduler.
        scheduler.Stop();
        // Method to end the scheduler process.
        // When a scheduler is finished, it's stopped, the state of that is set on finished, and the finishing callbacks are called.
        // It has a parameter that specifies a custom text for finishing that is accessible in the Finish callback.
        scheduler.Finish("Custom request for finishing!");
        // Method to dispose
        scheduler.Dispose();
    }
}
```

# Functional Examples
### In continuation, some functional examples and their descriptions are implemented so you can be more familiar with this library.
### In the first step, a simple scheduler implementation for showing the date time per second:
```csharp
var scheduler = new G9Scheduler()
    .AddSchedulerAction(s => 
        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))
    .SetDurationPeriodBetweenExecutions(TimeSpan.FromSeconds(1))
    .Start();

// 2022-09-09 19:09:37
// 2022-09-09 19:09:38
// 2022-09-09 19:09:39
```
### A scheduler is implemented for performing the desired task with three tries. Indeed, in this implementation, the scheduler runs the specified action a maximum of three times. In each round, if the scheduler can do its task, the finishing method with custom text is run, and the job is done. So, if it was successful, expect the specified custom text received in the finish callback method. Otherwise, after three tries, the finishing method will be called automatically, and we know it was unsuccessful by paying attention to the specified reason and text.
```csharp
private static void Main()
{
    var scheduler = new G9Scheduler()
    .AddSchedulerAction(s =>
    {
        // To do something ...
        // If it's successful, the finish method is run.
        s.Finish(
            // Specifies an explanation text for the finish reason.
            // The specified text passes to the related callback.
            "The main task of the scheduler is finished.");
    })
    // Otherwise, after three times trying, the scheduler will be finished.
    .SetCountOfRepetitions(3, G9ERepetitionConditionType.InTotal)
    .AddFinishCallback((sc, reason, text) =>
    {
        // Result
        Console.WriteLine(
            reason == G9EFinishingReason.FinishedByCustomRequest &&
            text == "The main task of the scheduler is finished."
            ? "Task is done successfully"
            : "The task isn't done.");
    })
    .Start();
}
```
### Suppose we want to run the above example for each day between a specified date. In that case, we must change the method's second parameter, "SetCountOfRepetitions," to "G9ERepetitionConditionType.PerDay" and add a start and end date time condition for that, like below:
```csharp
private static void Main()
{
    var scheduler = new G9Scheduler()
    .AddSchedulerAction(s =>
    {
        // To do something ...
        // If it's successful, the finish method is run.
        s.Finish(
            // Specifies an explanation text for the finish reason.
            // The specified text passes to the related callback.
            "The main task of the scheduler is finished.");
    })
    // Otherwise, after three times trying, the scheduler will be finished.
    .SetCountOfRepetitions(3, G9ERepetitionConditionType.PerDay)
    .SetStartDateTime(DateTime.Now)
    .SetEndDateTime(DateTime.Now.AddMonths(1))
    .AddFinishCallback((sc, reason, text) =>
    {
        // Result
        Console.WriteLine(
            reason == G9EFinishingReason.FinishedByCustomRequest &&
            text == "The main task of the scheduler is finished."
            ? "Task is done successfully"
            : "The task isn't done.");
    })
    .Start();
}
```
### Implementation of a scheduler to run from 10 AM to 2 PM with a one-hour duration gap between each execution:
```csharp
var scheduler = new G9Scheduler()
    .AddSchedulerAction(s =>
        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))
    .SetStartTime(new G9DtTime(10, 0, 0))
    // If needed to run on the last hour, the last hour must be set greater than 14:00:00
    .SetEndTime(new G9DtTime(14, 0, 1))
    .SetDurationPeriodBetweenExecutions(TimeSpan.FromHours(1))
    .Start();

// 2022-09-09 10:00:00
// 2022-09-09 11:00:00
// 2022-09-09 12:00:00
// 2022-09-09 13:00:00
// 2022-09-09 14:00:00
```
### A scheduler with a custom condition:
```csharp
var desiredExecutionTime = DateTime.Now.AddSeconds(9);
var scheduler = new G9Scheduler()
    .AddSchedulerAction(s =>
    {
        Console.WriteLine("Happy birthday!");
        s.Finish("The task is done!");
    })
    .AddCondition(s => DateTime.Now >= desiredExecutionTime)
    .Start();
```
# Static methods for creating custom events
### This library also provides valuable static methods for creating an exciting scheduler that works like an event!
### Implementation of a custom event that executes when the 'BackgroundColor' is set to 'DarkMagenta':
```csharp
var scheduler = G9Scheduler
    .GenerateCustomEvent(
        // Specifies the condition of the custom event.
        sh =>
            Console.BackgroundColor == ConsoleColor.DarkMagenta,
        // Specifies the action of the custom event that will be run if the condition is true.
        sh =>
        {
            Console.WriteLine("The 'BackgroundColor' is 'DarkMagenta'");
            sh.Dispose();
        })
    .Start();

Console.BackgroundColor = ConsoleColor.Black;
// The created event runs on this change
Console.BackgroundColor = ConsoleColor.DarkMagenta;
// The 'BackgroundColor' is 'DarkMagenta'
```

### Implementation of a custom event that is sensitive to the value and executes if the change occurs in the specified member value:
#### Assumed there is a class like the below:
```csharp
internal class Sample
{
    public int Age;
}
```
#### The implementation for recognizing the change in this class would be like the below:
```csharp
// Creates an instance
var sample = new Sample();

var scheduler = G9Scheduler
    .GenerateCustomEventOnValueChange(
        // Specifies the target object for access to its members.
        sample,
        // Specifies the desired member of the object for checking and recognizing a change in its value.
        s => s.Age,
        // Specifies an action for accessing the old value and the new value of the desired member.
        (oldValue, newValue) =>
        {
            Console.WriteLine($"The old value is '{oldValue},' and the new value is '{newValue}.'");
        },
        // Specifies a custom duration for checking; it's set to one second by default.
        TimeSpan.FromMilliseconds(50))
    .Start();

sample.Age = 32;
Thread.Sleep(100);
sample.Age = 99;

// The old value is '0,' and the new value is '32.'
// The old value is '32,' and the new value is '99.'
```


# Important Points
- ### The created object of the scheduler must store in a lifetime variable (like a static variable or a variable in the 'program.cs' file). If the created object is defined in a limited scope and the process of that is finished. The core disposes of the scheduler automatically.
- ### In web-based projects, even if storing the scheduler object performs in a static variable, the core still disposes of the scheduler because static variables in the web-based project have a limited lifetime. So, the storing process in this type of project must perform in some scopes that have a lifetime process (In the new .NET web app can perform in the 'program.cs' file, and in the older version can define in 'Global.asax').