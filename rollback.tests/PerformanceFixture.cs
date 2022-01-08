using System;
using NUnit.Framework;

namespace Rollback.Tests
{
    [TestFixture]
    public class PerformanceFixture
    {
        private int _startTime;
        private int _iterations;

        [SetUp]
        public void SetUp()
        {
            _startTime = Environment.TickCount;
        }

        protected void ReportIterations(int iterations)
        {
            _iterations = iterations;
        }

        [TearDown]
        public void TearDown()
        {
            var name = TestContext.CurrentContext.Test.FullName;
            var timeEnd = Environment.TickCount;
            var timePassed = timeEnd - _startTime;
            var timePassedNanos = timePassed * 1_000_000;
            var timePassedNanosPerIteration = timePassedNanos / _iterations;
            Console.Out.WriteLine("{0}: {1} nanos  ({2} millis over {3} iterations)", name, timePassedNanosPerIteration,
                timePassed, _iterations);
        }
    }
}