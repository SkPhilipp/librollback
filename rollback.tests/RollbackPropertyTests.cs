using Fuzzer;
using Fuzzer.blueprint;
using NUnit.Framework;
using Rollback.structures;
using Rollback.tests;

namespace Rollback.Tests.structures
{
    public class RollbackPropertyFuzzerContext : RollbackFuzzerContext<RollbackProperty<double>>
    {
        public RollbackPropertyFuzzerContext() : base(new RollbackProperty<double>(0), new RollbackProperty<double>(0))
        {
        }

        public void StepIncrement(double seed)
        {
            if (seed > 0.5)
            {
                Time++;
            }
            Main.Set(Time, Main.Get() + seed);
            if (Mode == ExecutionMode.Dual)
            {
                Control.Set(Time, Control.Get() + seed);
            }
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
            var fuzzer = new Fuzzer<RollbackPropertyFuzzerContext>(() => new RollbackPropertyFuzzerContext());
            fuzzer.Fuzz(blueprint, 500);
        }
    }
}