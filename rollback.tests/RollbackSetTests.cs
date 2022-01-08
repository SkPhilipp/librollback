using Fuzzer;
using Fuzzer.blueprint;
using Fuzzer.core;
using NUnit.Framework;
using Rollback.structures;
using Rollback.tests;

namespace Rollback.Tests.structures
{
    public class RollbackSetFuzzerContext : RollbackFuzzerContext<RollbackSet<double>>
    {
        public RollbackSetFuzzerContext() : base(new RollbackSet<double>(), new RollbackSet<double>())
        {
        }

        public void StepAdd(double seed)
        {
            if (seed > 0.5)
            {
                Time++;
            }
            var value = FuzzerUtils.SeedToInt(seed, 10);
            Main.Add(Time, value);
            if (Mode == ExecutionMode.Dual)
            {
                Control.Add(Time, value);
            }
        }

        public void StepRemove(double seed)
        {
            if (seed > 0.5)
            {
                Time++;
            }
            var value = FuzzerUtils.SeedToInt(seed, 10);
            Main.Remove(Time, value);
            if (Mode == ExecutionMode.Dual)
            {
                Control.Remove(Time, value);
            }
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
            var fuzzer = new Fuzzer<RollbackSetFuzzerContext>(() => new RollbackSetFuzzerContext());
            fuzzer.Fuzz(blueprint, 500);
        }
    }
}