using System;
using System.Threading;
using G9AssemblyManagement;
using G9ScheduleManagement;
using G9ScheduleManagement.DataType;
using G9ScheduleManagement.Enum;
using G9ScheduleNUnitTest.DataType;
using NUnit.Framework;

namespace G9ScheduleNUnitTest
{
    public class G9CScheduleManagementNUnitTest
    {
        //#if NET35
        //        private readonly bool _isDotNet35 = true;
        //#else
        //        private readonly bool _isDotNet35 = false;
        //#endif
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Order(1)]
        public void TestBasicScheduler()
        {
            var counter = 0;
            var schedule = new G9Scheduler()
                .AddSchedulerAction(s => { counter++; })
                .SetDurationPeriodBetweenExecutions(G9DtGap.OneSec)
                .AddErrorCallback((s, exception) => { Assert.Fail($"Schedule error!\n{exception.StackTrace}"); })
                .Start();

            Thread.Sleep(3099);
            if (counter < 3)
                Assert.Fail("Fail run");
        }

        [Test]
        [Order(2)]
        public void TestAdvancedScheduler()
        {
            // Flags for testing
            var errorCheck = false;
            var conditionCheck = false;
            var onDisposeCheck = false;
            var onFinishCheck = false;
            var onStopCheck = false;
            var onStartCheck = false;
            var onPreExecution = false;
            var onEndExecution = false;

            var specifiedStartDateTime = DateTime.Now.AddSeconds(3);
            var specifiedEndDateTime = DateTime.Now.AddSeconds(16);

            var counter = 0;
            var schedule = new G9Scheduler();
            schedule
                .AddSchedulerAction(s =>
                {
                    // Check time of first execution.
                    if (DateTime.Now < specifiedStartDateTime)
                        Assert.Fail("Fail on first execution condition.");

                    counter++;

                    // Thrown custom exception after 6 second - for testing the process of error callback
                    if (counter == 6)
                        throw new Exception("It's okay");
                })
                .AddErrorCallback((s, ex) =>
                {
                    errorCheck = true;
                    Assert.True(ex.Message == "It's okay");
                })
                .AddCondition(s =>
                {
                    conditionCheck = true;
                    return true;
                })
                .AddDisposeCallback((s, reason) =>
                {
                    onDisposeCheck = true;
                    Assert.True(reason == G9EDisposeReason.DisposedByMethod);
                })
                .AddFinishCallback((s, reason, text) =>
                {
                    Assert.True(reason == G9EFinishingReason.FinishedByEndDateTimeCondition);
                    onFinishCheck = true;
                })
                .AddStopCallback(s => onStopCheck = true)
                .AddStartCallback(s => onStartCheck = true)
                // Pre && End execution
                .AddPreExecutionCallback(s => onPreExecution = true)
                .AddEndExecutionCallback(s => onEndExecution = true)
                .SetDurationPeriodBetweenExecutions(G9DtGap.OneSec)
                // The first execution must be done after 3 seconds
                .SetStartDateTime(specifiedStartDateTime)
                .SetEndDateTime(specifiedEndDateTime);

            Assert.True(schedule.SchedulerState == G9ESchedulerState.InitializedState);

            // Start
            schedule.Start();
            Assert.True(schedule.SchedulerState == G9ESchedulerState.StartedStateWithoutExecution);

            Thread.Sleep(3369);

            // After first execution

            Assert.True(schedule.SchedulerState == G9ESchedulerState.StartedStateOnPreExecution ||
                        schedule.SchedulerState == G9ESchedulerState.StartedStateOnEndExecution ||
                        schedule.SchedulerState == G9ESchedulerState.ConditionalRejectExecution);

            // Testing the process of stop callback
            schedule.Stop();
            Assert.True(schedule.SchedulerState == G9ESchedulerState.PausedState);

            // Resumed
            schedule.Start();
            Assert.True(schedule.SchedulerState == G9ESchedulerState.StartedStateWithoutExecution ||
                        schedule.SchedulerState == G9ESchedulerState.StartedStateOnPreExecution ||
                        schedule.SchedulerState == G9ESchedulerState.StartedStateOnEndExecution ||
                        schedule.SchedulerState == G9ESchedulerState.ConditionalRejectExecution);

            // Wait for finishing
            while (schedule.SchedulerState != G9ESchedulerState.FinishedState) Thread.Sleep(39);

            // Finished
            Assert.True(schedule.SchedulerState == G9ESchedulerState.FinishedState);

            // Testing the process of disposing
            schedule.Dispose();

            // Checking the whole situation
            Assert.True(errorCheck && conditionCheck && onDisposeCheck && onFinishCheck && onStopCheck &&
                        onStartCheck && onPreExecution && onEndExecution);
        }

        [Test]
        [Order(3)]
        public void TestSchedulerWithCustomCondition()
        {
            var specifiedDateTime = DateTime.Now.AddSeconds(3);

            // Flags
            var executionTime = DateTime.MinValue;

            var schedule =
                new G9Scheduler()
                    .AddSchedulerAction(s => { executionTime = DateTime.Now; })
                    .AddCondition(s => DateTime.Now > specifiedDateTime)
                    .SetCountOfRepetitions(1, G9ERepetitionConditionType.InTotal)
                    .Start();


            // Wait for finishing
            while (schedule.SchedulerState != G9ESchedulerState.FinishedState) Thread.Sleep(39);

            Assert.True(executionTime > specifiedDateTime);
        }

        [Test]
        [Order(4)]
        public void TestSchedulerAsEvents()
        {
            var specifiedDateTime = DateTime.Now.AddSeconds(3);

            var execution = false;

            // Scheduler as a event on condition
            var testSchedulerAsEvent = G9Scheduler.GenerateCustomEvent(s => DateTime.Now > specifiedDateTime,
                    s =>
                    {
                        Assert.True(DateTime.Now > specifiedDateTime);
                        execution = true;
                    })
                .Start();

            // Wait for execution
            while (!execution) Thread.Sleep(39);

            Assert.True(execution);
            testSchedulerAsEvent.Dispose();

            // Scheduler as a event on change value
            var testClass = new G9DtSimpleClass();
            execution = false;
            var currentName = testClass.Name;
            var nextName = testClass.Name + "MT9G";
            var testSchedulerAsEventOnValueChange = G9Scheduler.GenerateCustomEventOnValueChange(testClass, s => s.Name,
                    (oldName, newName) =>
                    {
                        Assert.True(oldName == currentName && newName == nextName);
                        execution = true;
                    })
                .Start();
            testClass.Name = nextName;

            // Wait for execution
            while (testSchedulerAsEventOnValueChange.SchedulerState != G9ESchedulerState.StartedStateOnPreExecution &&
                   testSchedulerAsEventOnValueChange.SchedulerState != G9ESchedulerState.StartedStateOnEndExecution)
                Thread.Sleep(39);
            Assert.True(execution);
            testSchedulerAsEventOnValueChange.Dispose();
        }

        [Test]
        [Order(5)]
        public void TestTimeCondition()
        {
            var currentDateTime = DateTime.Now;
            DateTime lastRunDateTime = default;
            var startTime = G9DtTime.ParseTimeSpan(currentDateTime.TimeOfDay.Add(new TimeSpan(0, 0, 3)));
            var endTime = G9DtTime.ParseTimeSpan(currentDateTime.TimeOfDay.Add(new TimeSpan(0, 0, 9)));

            var scheduler = new G9Scheduler()
                .AddSchedulerAction(s =>
                {
                    var runningDateTime = DateTime.Now;
                    Assert.True(runningDateTime.TimeOfDay > startTime.ConvertToTimeSpan() &&
                                runningDateTime.TimeOfDay < endTime.ConvertToTimeSpan());
                    lastRunDateTime = DateTime.Now;
                })
                .SetStartTime(startTime)
                .SetEndTime(endTime)
                .Start();

            Thread.Sleep(12_000);

            scheduler.Dispose();

            Assert.True(currentDateTime.AddSeconds(6) <= lastRunDateTime);
        }

        [Test]
        [Order(6)]
        public void TestExceptions()
        {
            var scheduler = new G9Scheduler();

            // Checks for starting without the scheduled action
            try
            {
                scheduler.Start();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message ==
                            $"The scheduler for starting needs to have at least one added schedule action. For adding, you can use the method '{nameof(G9Scheduler.AddSchedulerAction)}.'");
            }

            // Checks for adding a null item for the scheduled action
            try
            {
                scheduler.AddSchedulerAction(null);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message.StartsWith(
                    "The specified item for adding cannot be null (here is 'schedulerAction')"));
            }

            // Checks for removing an item (here is a condition) that does not exist (not added for this scheduler)
            try
            {
                scheduler.RemoveCondition(s => true);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message.StartsWith(
                    "No items have been added to this scheduler, so you cannot remove a specified action (here is 'customConditionFunc') from it."));
            }

            // Checks for stopping a scheduler that isn't started.
            try
            {
                scheduler.Stop();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message == "The scheduler had already been stopped, and now it can't stop again.");
            }
        }

        [Test]
        [Order(7)]
        public void TestAddingAndRemovingThings()
        {
            // Set callbacks and function
            Func<G9Scheduler, bool> condition = s => true;
            Action<G9Scheduler> schedulerAction = s => { TestContext.WriteLine("schedulerAction"); };
            Action<G9Scheduler> startCallback = s => { TestContext.WriteLine("startCallback"); };
            Action<G9Scheduler> stopCallback = s => { TestContext.WriteLine("stopCallback"); };
            Action<G9Scheduler, G9EFinishingReason, string> finishCallback = (s, reason, text) =>
            {
                TestContext.WriteLine($"finishCallback: {reason} | text: {text}");
                Assert.True(reason == G9EFinishingReason.FinishedByCustomRequest &&
                            text == "Custom request for finishing!");
            };
            Action<G9Scheduler, Exception> errorCallback = (s, ex) =>
            {
                TestContext.WriteLine($"errorCallback: {ex.Message}");
            };
            Action<G9Scheduler, G9EDisposeReason> disposeCallback = (s, reason) =>
            {
                TestContext.WriteLine($"disposeCallback: {reason}");
            };

            // Initialize
            var scheduler = new G9Scheduler();

            // Set callbacks - conditions - etc
            scheduler
                .AddSchedulerAction(schedulerAction)
                .AddCondition(condition)
                .AddStartCallback(startCallback)
                .AddStopCallback(stopCallback)
                .AddFinishCallback(finishCallback)
                .AddErrorCallback(errorCallback)
                .AddDisposeCallback(disposeCallback)
                .SetStartDateTime(DateTime.Now)
                .SetEndDateTime(DateTime.Now.AddMonths(1))
                .SetStartTime(G9DtTime.FromHours(6))
                .SetEndTime(G9DtTime.FromHours(16))
                .SetCountOfRepetitions(99, G9ERepetitionConditionType.PerDay)
                .SetDurationPeriodBetweenExecutions(G9DtGap.OneSec);

            // Starting
            scheduler.Start();

            Thread.Sleep(1999);

            // Remove callbacks
            scheduler
                .RemoveCondition(condition)
                .RemoveStartCallback(startCallback)
                .RemoveStopCallback(stopCallback)
                .RemoveFinishCallback(finishCallback)
                .RemoveErrorCallback(errorCallback)
                .RemoveDisposeCallback(disposeCallback);

            // Set (update) conditions
            scheduler
                .SetStartDateTime(DateTime.Now.AddSeconds(3))
                .SetEndDateTime(DateTime.Now.AddMonths(2))
                .SetStartTime(G9DtTime.FromHours(8))
                .SetEndTime(G9DtTime.FromHours(12))
                .SetCountOfRepetitions(999, G9ERepetitionConditionType.PerDay)
                .SetDurationPeriodBetweenExecutions(G9DtGap.Init(0, 0, 0, 0, 500));

            Thread.Sleep(339);

            // Set callbacks
            scheduler
                .AddSchedulerAction(schedulerAction)
                .AddCondition(condition)
                .AddStartCallback(startCallback)
                .AddStopCallback(stopCallback)
                .AddFinishCallback(finishCallback)
                .AddErrorCallback(errorCallback)
                .AddDisposeCallback(disposeCallback);

            Thread.Sleep(1999);

            // An exception is expected on removing the scheduler action in runtime.
            // A scheduler can't work without any scheduler action.
            try
            {
                scheduler.RemoveSchedulerAction(schedulerAction);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message == "A scheduler can't work without any scheduler action.");
            }

            // The previous exception doesn't occur on stopping time, and it's changeable.
            scheduler.Finish("Custom request for finishing!");
            scheduler.Stop();
            scheduler.RemoveSchedulerAction(schedulerAction);
            scheduler.AddSchedulerAction(schedulerAction);
            scheduler.Start();

            Thread.Sleep(1999);

            scheduler.Dispose();

            Thread.Sleep(999);
        }

        [Test]
        [Order(8)]
        public void TestSchedulerQueue()
        {
            var counter = 0;

            // Test with queue
            var scheduler = new G9Scheduler()
                .AddSchedulerAction(s =>
                {
                    Thread.Sleep(1000);
                    counter++;
                })
                .Start();

            Thread.Sleep(1369);
            Assert.True(counter == 1);
            TestContext.WriteLine($"Counter (With Queue): {counter}");
            scheduler.Dispose();

            // Test without queue
            counter = 0;
            scheduler = new G9Scheduler()
                .AddSchedulerAction(s =>
                {
                    Thread.Sleep(1000);
                    counter++;
                })
                // Disable queue
                .SetQueueMode(false)
                .Start();

            Thread.Sleep(1369);
            Assert.True(counter > 1);
            TestContext.WriteLine($"Counter (Without Queue): {counter}");
            scheduler.Dispose();
        }

        [Test]
        [Order(9)]
        public void TestSchedulerTries()
        {

            try
            {
                new G9Scheduler()
                    .SetCountOfTries(10);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.True(e.Message == $"The count of tries can't be set before or without specifying the count of repetitions (With method '{nameof(G9Scheduler.SetCountOfRepetitions)}'). Because trying without repetition limit doesn't work.");
            }

            var counter = 0;
            bool finish = false;
            bool finishingCallBack = false;
            var scheduler = new G9Scheduler()
                .AddSchedulerAction(s =>
                {
                    TestContext.WriteLine(DateTime.Now.ToString("O"));
                    counter++;
                    if (counter < 3)
                        throw new Exception("Fake Exception!");
                    finish = true;
                })
                .SetCountOfRepetitions(1, G9ERepetitionConditionType.InTotal)
                .SetCountOfTries(2, G9DtGap.FromMilliseconds(500))
                .Start();

            Thread.Sleep(1009);
            Assert.True(finish && finishingCallBack);
        }
    }
}