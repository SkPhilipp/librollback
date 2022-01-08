using Fuzzer;
using Fuzzer.blueprint;
using NUnit.Framework;
using Rollback.structures;
using Rollback.tests;

namespace Rollback.Tests.structures
{
    public class RollbackListFuzzerContext : RollbackFuzzerContext<RollbackList<double>>
    {
        public RollbackListFuzzerContext() : base(new RollbackList<double>(), new RollbackList<double>())
        {
        }

        public void StepPush(double seed)
        {
            if (seed > 0.5)
            {
                Time++;
            }
            Main.Push(Time, new[] { seed });
            if (Mode == ExecutionMode.Dual)
            {
                Control.Push(Time, new[] { seed });
            }
        }

        public void StepPop(double seed)
        {
            if (seed > 0.5)
            {
                Time++;
            }
            Main.Pop(Time);
            if (Mode == ExecutionMode.Dual)
            {
                Control.Pop(Time);
            }
        }

        public void StepSet(double seed)
        {
            if (seed > 0.5)
            {
                Time++;
            }
            var index = (int)(seed * 10);
            Main.Set(Time, index, seed);
            if (Mode == ExecutionMode.Dual)
            {
                Control.Set(Time, index, seed);
            }
        }
    }

    [TestFixture]
    public class RollbackListFuzz
    {
        [Test]
        public void Fuzz()
        {
            var blueprint = new FuzzerBlueprint<RollbackListFuzzerContext>()
                .Phase(0, 20)
                .Step("StepPush", (context, seed) => context.StepPush(seed))
                .Step("StepPop", (context, seed) => context.StepPop(seed))
                .Step("StepSet", (context, seed) => context.StepSet(seed))
                .Phase(1, 1)
                .Step("StepRollbackPlan", (context, _) => context.StepRollbackPlan())
                .Phase(0, 20)
                .Step("StepPush", (context, seed) => context.StepPush(seed))
                .Step("StepPop", (context, seed) => context.StepPop(seed))
                .Step("StepSet", (context, seed) => context.StepSet(seed))
                .Step("StepRollbackPerform", (context, _) => context.StepRollbackPerform())
                .Phase(1, 1)
                .Step("StepComplete", (context, _) => context.StepComplete());
            var fuzzer = new Fuzzer<RollbackListFuzzerContext>(() => new RollbackListFuzzerContext());
            fuzzer.Fuzz(blueprint, 500);
        }
    }
}