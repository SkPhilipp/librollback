using Fuzzer;
using Fuzzer.blueprint;
using Fuzzer.core;
using NUnit.Framework;
using Rollback.tests;

namespace Rollback.Tests
{
    public class RollbackSetFuzzerContext : RollbackFuzzerContext<RollbackSet<double>>
    {
        public RollbackSetFuzzerContext(RollbackClock clock) : base(clock, new RollbackSet<double>(clock), new RollbackSet<double>(clock))
        {
        }

        public void StepAdd(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            var value = FuzzerUtils.SeedToInt(seed, 10);
            Apply(list => list.Add(value));
        }

        public void StepRemove(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            var value = FuzzerUtils.SeedToInt(seed, 10);
            Apply(list => list.Remove(value));
        }
    }

    [TestFixture]
    public class RollbackSetFuzz
    {
        [Test]
        public void Fuzz()
        {
            var blueprint = new FuzzerBlueprint<RollbackSetFuzzerContext>()
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
            var fuzzer = new Fuzzer<RollbackSetFuzzerContext>(() => new RollbackSetFuzzerContext(new RollbackClock()));
            fuzzer.Fuzz(blueprint, 500);
        }
    }
}