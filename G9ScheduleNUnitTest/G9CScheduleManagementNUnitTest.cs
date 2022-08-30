using System;
using System.Threading;
using G9ScheduleManagement;
using NUnit.Framework;

namespace G9ScheduleNUnitTest
{
    public class G9CScheduleManagementNUnitTest
    {
        private G9CSchedule _schedule;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Order(1)]
        public void InitializeG9Schedule()
        {
            _schedule = new G9CSchedule();
        }

        [Test]
        [Order(2)]
        public void TestScheduleRunPerSecond()
        {
            var counter = 0;
            _schedule
                .AddScheduleAction(() => { counter++; })
                .SetDuration(TimeSpan.FromSeconds(1))
                .AddErrorCallBack(exception => { Assert.Fail($"Schedule error!\n{exception.StackTrace}"); });

            TestContext.WriteLine($"Sleep Start DateTime: {DateTime.Now:s}");
            Thread.Sleep(3369);
            TestContext.WriteLine($"Sleep End DateTime: {DateTime.Now:s}");
            TestContext.WriteLine($"The count of number is: {counter}");
            if (counter >= 3)
                Assert.Pass();
            else
                Assert.Fail("Fail run");
            _schedule.Dispose();
        }
    }
}