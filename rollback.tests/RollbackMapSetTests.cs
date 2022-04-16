using Fuzzer;
using Fuzzer.blueprint;
using Fuzzer.core;
using NUnit.Framework;
using Rollback.tests;

namespace Rollback.Tests
{
    public class RollbackSetMapFuzzerContext : RollbackFuzzerContext<RollbackSetMap<int, int>>
    {
        public RollbackSetMapFuzzerContext(RollbackClock clock) : base(clock, new RollbackSetMap<int, int>(clock), new RollbackSetMap<int, int>(clock))
        {
        }

        public void StepAdd(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            var key = FuzzerUtils.SeedToInt(seed, 4);
            var value = FuzzerUtils.SeedToInt(seed, 10);
            Apply(list => list.Add(key, value));
        }

        public void StepRemove(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            var key = FuzzerUtils.SeedToInt(seed, 4);
            var value = FuzzerUtils.SeedToInt(seed, 10);
            Apply(list => list.Remove(key, value));
        }
    }

    [TestFixture]
    public class RollbackSetMapFuzz
    {
        [Test]
        public void Fuzz()
        {
            var blueprint = new FuzzerBlueprint<RollbackSetMapFuzzerContext>()
                .Phase(0, 20)
                .Step("StepAdd", (context, seed) => context.StepAdd(seed))
                .Step("StepRemove", (context, seed) => context.StepRemove(seed))
                .Phase(1, 1)
                .Step("StepRollbackPlan", (context, _) => context.StepRollbackPlan())
                .Phase(0, 20)
                .Step("StepAdd", (context, seed) => context.StepAdd(seed))
                .Step("StepRemove", (context, seed) => context.StepRemove(seed))
                .Step("StepRollbackPerform", (context, _) => context.StepRollbackPerform())
                .Phase(1, 1)
                .Step("StepComplete", (context, _) => context.StepComplete());
            var fuzzer = new Fuzzer<RollbackSetMapFuzzerContext>(() => new RollbackSetMapFuzzerContext(new RollbackClock()));
            fuzzer.Fuzz(blueprint, 5000);
        }
    }

    [TestFixture]
    public class RollbackSetMapTests
    {
        [Test]
        public void TestSorting()
        {
            var clock = new RollbackClock();
            var setMap = new RollbackSetMap<int, ComparableObjectExample>(clock);
            for (var i = 100; i < 200; i++)
            {
                setMap.Add(0, new ComparableObjectExample(i));
            }

            for (var i = 0; i < 100; i++)
            {
                setMap.Add(0, new ComparableObjectExample(i));
            }

            var expected = 0;
            foreach (var comparableObjectExample in setMap[0])
            {
                Assert.AreEqual(expected, comparableObjectExample.Order);
                expected++;
            }
        }
    }
}