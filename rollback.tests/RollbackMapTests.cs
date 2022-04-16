using Fuzzer;
using Fuzzer.blueprint;
using Fuzzer.core;
using NUnit.Framework;
using Rollback.tests;

namespace Rollback.Tests
{
    public class RollbackMapFuzzerContext : RollbackFuzzerContext<RollbackMap<double, double>>
    {
        public RollbackMapFuzzerContext(RollbackClock clock) : base(clock, new RollbackMap<double, double>(clock), new RollbackMap<double, double>(clock))
        {
        }

        public void StepAdd(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }


            var value = FuzzerUtils.SeedToInt(seed, 10);
            Apply(list => list.Set(value, seed));
        }

        public void StepRemove(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            var key = FuzzerUtils.SeedToInt(seed, 10);
            Apply(list => list.Remove(key));
        }
    }

    [TestFixture]
    public class RollbackMapFuzz
    {
        [Test]
        public void Fuzz()
        {
            var blueprint = new FuzzerBlueprint<RollbackMapFuzzerContext>()
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
            var fuzzer = new Fuzzer<RollbackMapFuzzerContext>(() => new RollbackMapFuzzerContext(new RollbackClock()));
            fuzzer.Fuzz(blueprint, 5000);
        }
    }
}