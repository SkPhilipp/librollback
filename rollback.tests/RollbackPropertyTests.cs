using Fuzzer;
using Fuzzer.blueprint;
using NUnit.Framework;
using Rollback.tests;

namespace Rollback.Tests
{
    public class RollbackPropertyFuzzerContext : RollbackFuzzerContext<RollbackProperty<double>>
    {
        public RollbackPropertyFuzzerContext(RollbackClock clock) : base(clock, new RollbackProperty<double>(clock, 0), new RollbackProperty<double>(clock, 0))
        {
        }

        public void StepIncrement(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            Apply(list => list.Set(list.Get() + seed));
        }
    }

    [TestFixture]
    public class RollbackPropertyFuzz
    {
        [Test]
        public void Fuzz()
        {
            var blueprint = new FuzzerBlueprint<RollbackPropertyFuzzerContext>()
                .Phase(0, 20)
                .Step("StepIncrement", (context, seed) => context.StepIncrement(seed))
                .Phase(1, 1)
                .Step("StepRollbackPlan", (context, _) => context.StepRollbackPlan())
                .Phase(0, 20)
                .Step("StepIncrement", (context, seed) => context.StepIncrement(seed))
                .Step("StepRollbackPerform", (context, _) => context.StepRollbackPerform())
                .Phase(1, 1)
                .Step("StepComplete", (context, _) => context.StepComplete());
            var fuzzer = new Fuzzer<RollbackPropertyFuzzerContext>(() => new RollbackPropertyFuzzerContext(new RollbackClock()));
            fuzzer.Fuzz(blueprint, 500);
        }
    }
}