using NUnit.Framework;

namespace Rollback.Tests
{
    [TestFixture]
    public class RollbackListBenchmarks : PerformanceFixture
    {
        private const int IterationsOperations = 1_000_000;

        [Test]
        public void BenchmarkConstructor()
        {
            ReportIterations(IterationsOperations);
            var rollbackClock = new RollbackClock();
            var rollbackList = new RollbackList<int>(rollbackClock);
            for (var i = 1; i < IterationsOperations; i++)
            {
                rollbackList = new RollbackList<int>(rollbackClock);
            }

            Assert.IsNotNull(rollbackList.Count);
        }

        [Test]
        public void BenchmarkSet0()
        {
            ReportIterations(IterationsOperations);
            var rollbackClock = new RollbackClock();
            var rollbackList = new RollbackList<int>(rollbackClock);
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackList.Set(i, i);
            }

            rollbackClock.MoveTo(0);
            rollbackList.Rollback();
            Assert.IsNotNull(rollbackList.Count);
        }

        [Test]
        public void BenchmarkSetI()
        {
            ReportIterations(IterationsOperations);
            var rollbackClock = new RollbackClock();
            var rollbackList = new RollbackList<int>(rollbackClock);
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackClock.Tick();
                rollbackList.Set(i, i);
            }

            rollbackClock.MoveTo(0);
            rollbackList.Rollback();
            Assert.IsNotNull(rollbackList.Count);
        }

        [Test]
        public void BenchmarkPushPop0()
        {
            ReportIterations(IterationsOperations);
            var rollbackClock = new RollbackClock();
            var rollbackList = new RollbackList<int>(rollbackClock);
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackList.Push (new[] { i });
                rollbackList.Pop();
            }

            rollbackClock.MoveTo(0);
            rollbackList.Rollback();
            Assert.IsNotNull(rollbackList.Count);
        }

        [Test]
        public void BenchmarkPushPopI()
        {
            ReportIterations(IterationsOperations);
            var rollbackClock = new RollbackClock();
            var rollbackList = new RollbackList<int>(rollbackClock);
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackClock.Tick();
                rollbackList.Push(new[] { i });
                rollbackList.Pop();
            }

            rollbackClock.MoveTo(0);
            rollbackList.Rollback();
            Assert.IsNotNull(rollbackList.Count);
        }
    }
}