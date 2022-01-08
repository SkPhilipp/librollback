using NUnit.Framework;
using Rollback.structures;

namespace Rollback.Tests.structures
{
    [TestFixture]
    public class RollbackListBenchmarks : PerformanceFixture
    {
        private const int IterationsOperations = 1_000_000;

        [Test]
        public void BenchmarkConstructor()
        {
            ReportIterations(IterationsOperations);
            var rollbackList = new RollbackList<int>();
            for (var i = 1; i < IterationsOperations; i++)
            {
                rollbackList = new RollbackList<int>();
            }

            rollbackList.Length();
        }

        [Test]
        public void BenchmarkSet0()
        {
            ReportIterations(IterationsOperations);
            var rollbackList = new RollbackList<int>();
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackList.Set(0, i, i);
            }

            rollbackList.Rollback(0);
        }

        [Test]
        public void BenchmarkSetI()
        {
            ReportIterations(IterationsOperations);
            var rollbackList = new RollbackList<int>();
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackList.Set(i, i, i);
            }

            rollbackList.Rollback(0);
        }

        [Test]
        public void BenchmarkPushPop0()
        {
            ReportIterations(IterationsOperations);
            var rollbackList = new RollbackList<int>();
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackList.Push(0, new[] { i });
                rollbackList.Pop(0);
            }

            rollbackList.Rollback(0);
        }

        [Test]
        public void BenchmarkPushPopI()
        {
            ReportIterations(IterationsOperations);
            var rollbackList = new RollbackList<int>();
            for (var i = 0; i < IterationsOperations; i++)
            {
                rollbackList.Push(i, new[] { i });
                rollbackList.Pop(i);
            }

            rollbackList.Rollback(0);
        }
    }
}