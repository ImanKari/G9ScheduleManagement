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
                .AddScheduleAction(() => { counter++; })
                .SetPeriodDurationBetweenExecutions(TimeSpan.FromSeconds(1))
                .AddErrorCallback(exception => { Assert.Fail($"Schedule error!\n{exception.StackTrace}"); })
                .StartOrResume();

            // Wait for finishing
            while (schedule.SchedulerState != G9ESchedulerState.StartedWithExecution) Thread.Sleep(39);
            Thread.Sleep(3999);
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
            var onResumeCheck = false;
            var onStartCheck = false;

            var specifiedStartDateTime = DateTime.Now.AddSeconds(3);
            var specifiedEndDateTime = DateTime.Now.AddSeconds(16);

            var counter = 0;
            var schedule = new G9Scheduler();
            schedule
                .AddScheduleAction(() =>
                {
                    // Check time of first execution.
                    if (DateTime.Now < specifiedStartDateTime)
                        Assert.Fail("Fail on first execution condition.");

                    counter++;

                    // Thrown custom exception after 6 second - for testing the process of error callback
                    if (counter == 6)
                        throw new Exception("It's okay");
                })
                .AddErrorCallback(ex =>
                {
                    errorCheck = true;
                    Assert.True(ex.Message == "It's okay");
                })
                .AddCondition(() =>
                {
                    conditionCheck = true;
                    return true;
                })
                .AddDisposeCallback(reason =>
                {
                    onDisposeCheck = true;
                    Assert.True(reason == G9EDisposeReason.DisposedByMethod);
                })
                .AddFinishCallback(() => onFinishCheck = true)
                .AddStopCallback(() => onStopCheck = true)
                .AddResumeCallback(() => onResumeCheck = true)
                .AddStartCallback(() => onStartCheck = true)
                .SetPeriodDurationBetweenExecutions(TimeSpan.FromSeconds(1))
                // The first execution must be done after 3 seconds
                .SetStartDateTime(specifiedStartDateTime)
                .SetEndDateTime(specifiedEndDateTime);

            Assert.True(schedule.SchedulerState == G9ESchedulerState.Initialized);

            // Start
            schedule.StartOrResume();
            Assert.True(schedule.SchedulerState == G9ESchedulerState.StartedWithoutExecution);

            Thread.Sleep(3369);

            // After first execution

            Assert.True(schedule.SchedulerState == G9ESchedulerState.StartedWithExecution);

            // Testing the process of stop callback
            schedule.Stop();
            Assert.True(schedule.SchedulerState == G9ESchedulerState.Paused);

            // Resumed
            schedule.StartOrResume();
            Assert.True(schedule.SchedulerState == G9ESchedulerState.ResumedWithoutExecution ||
                        schedule.SchedulerState == G9ESchedulerState.ResumedWithExecution);

            // Wait for finishing
            while (schedule.SchedulerState != G9ESchedulerState.Finished) Thread.Sleep(39);

            // Finished
            Assert.True(schedule.SchedulerState == G9ESchedulerState.Finished);

            // Testing the process of disposing
            schedule.Dispose();

            // Checking the whole situation
            Assert.True(errorCheck && conditionCheck && onDisposeCheck && onFinishCheck && onStopCheck &&
                        onResumeCheck && onStartCheck);
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
                    .AddScheduleAction(() => { executionTime = DateTime.Now; })
                    .AddCondition(() => DateTime.Now > specifiedDateTime)
                    .SetCountOfRepetitions(1)
                    .StartOrResume();


            // Wait for finishing
            while (schedule.SchedulerState != G9ESchedulerState.Finished) Thread.Sleep(39);

            Assert.True(executionTime > specifiedDateTime);
        }

        [Test]
        [Order(4)]
        public void TestSchedulerAsEvents()
        {
            var specifiedDateTime = DateTime.Now.AddSeconds(3);

            var execution = false;

            // Scheduler as a event on condition
            var testSchedulerAsEvent = G9Scheduler.GenerateCustomEvent(() => DateTime.Now > specifiedDateTime,
                    () =>
                    {
                        Assert.True(DateTime.Now > specifiedDateTime);
                        execution = true;
                    })
                .StartOrResume();

            // Wait for execution
            while (testSchedulerAsEvent.SchedulerState != G9ESchedulerState.StartedWithExecution) Thread.Sleep(39);

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
                .StartOrResume();
            testClass.Name = nextName;

            // Wait for execution
            while (testSchedulerAsEventOnValueChange.SchedulerState != G9ESchedulerState.StartedWithExecution)
                Thread.Sleep(39);
            Assert.True(execution);
            testSchedulerAsEventOnValueChange.Dispose();
        }

        [Test]
        [Order(5)]
        public void TestMultiThread()
        {
            var counter = 0;
            G9Assembly.PerformanceTools.MultiThreadShockTest(i =>
                {
                    counter++;
                    TestContext.WriteLine($"Number: {counter} | DateTime: {DateTime.Now:O}");
                    TestBasicScheduler();
                    TestAdvancedScheduler();
                    TestSchedulerWithCustomCondition();
                    TestSchedulerAsEvents();
                },
                9
            );

            Thread.Sleep(16399);
        }

        [Test]
        [Order(6)]
        public void TestTimeCondition()
        {
            var currentDateTime = DateTime.Now;
            DateTime lastRunDateTime = default;
            var startTime = G9DtTime.ParseTimeSpan(currentDateTime.TimeOfDay.Add(new TimeSpan(0, 0, 3)));
            var endTime = G9DtTime.ParseTimeSpan(currentDateTime.TimeOfDay.Add(new TimeSpan(0, 0, 9)));

            var scheduler = new G9Scheduler()
                .AddScheduleAction(() =>
                {
                    var runningDateTime = DateTime.Now;
                    Assert.True(runningDateTime.TimeOfDay > startTime.ConvertToTimeSpan() &&
                                runningDateTime.TimeOfDay < endTime.ConvertToTimeSpan());
                    lastRunDateTime = DateTime.Now;
                })
                .SetStartTime(startTime)
                .SetEndTime(endTime)
                .StartOrResume();

            Thread.Sleep(12_000);

            scheduler.Dispose();

            Assert.True(currentDateTime.AddSeconds(6) <= lastRunDateTime);
        }

        [Test]
        [Order(7)]
        public void TestExceptions()
        {
            using (var scheduler = new G9Scheduler())
            {
                // Checks for starting without the scheduled action
                try
                {
                    scheduler.StartOrResume();
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    Assert.True(ex.Message ==
                                $"The scheduler for starting needs to have at least one added schedule action. For adding, you can use the method '{nameof(G9Scheduler.AddScheduleAction)}.'");
                }

                // Checks for adding a null item for the scheduled action
                try
                {
                    scheduler.AddScheduleAction(null);
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    Assert.True(ex.Message.StartsWith(
                        "The specified item for adding cannot be null (here is 'scheduleAction')"));
                }

                // Checks for removing an item (here is a condition) that does not exist (not added for this scheduler)
                try
                {
                    scheduler.RemoveCondition(() => true);
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
        }

        [Test]
        [Order(8)]
        public void TestAddingAndRemovingThings()
        {
            var scheduler = new G9Scheduler();
        }
    }
}