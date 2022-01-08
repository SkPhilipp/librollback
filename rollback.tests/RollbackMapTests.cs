using Fuzzer;
using Fuzzer.blueprint;
using Fuzzer.core;
using NUnit.Framework;
using Rollback.structures;
using Rollback.tests;

namespace Rollback.Tests.structures
{
    public class RollbackMapFuzzerContext : RollbackFuzzerContext<RollbackMap<double, double>>
    {
        public RollbackMapFuzzerContext() : base(new RollbackMap<double, double>(), new RollbackMap<double, double>())
        {
        }

        public void StepAdd(double seed)
        {
            if (seed > 0.5)
            {
                Time++;
            }

            var value = FuzzerUtils.SeedToInt(seed, 10);
            Main.Set(Time, value, value);
            if (Mode == ExecutionMode.Dual)
            {
                Control.Set(Time, value, value);
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
            var fuzzer = new Fuzzer<RollbackMapFuzzerContext>(() => new RollbackMapFuzzerContext());
            fuzzer.Fuzz(blueprint, 500);
        }
    }
}