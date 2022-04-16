using Fuzzer;
using Fuzzer.blueprint;
using NUnit.Framework;
using Rollback.tests;

namespace Rollback.Tests
{
    public class RollbackListFuzzerContext : RollbackFuzzerContext<RollbackList<double>>
    {
        public RollbackListFuzzerContext(RollbackClock clock) : base(clock, new RollbackList<double>(clock), new RollbackList<double>(clock))
        {
        }

        public void StepPush(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            Apply(list => list.Push(new[] { seed }));
        }

        public void StepPop(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            Apply(list => list.Pop());
        }

        public void StepSet(double seed)
        {
            if (seed > 0.5)
            {
                Clock.Tick();
            }

            var index = (int)(seed * 10);

            Apply(list => list.Set(index, seed));
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
            var fuzzer = new Fuzzer<RollbackListFuzzerContext>(() => new RollbackListFuzzerContext(new RollbackClock()));
            fuzzer.Fuzz(blueprint, 5000);
        }
    }
}