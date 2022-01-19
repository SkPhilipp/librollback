using Fuzzer;
using Fuzzer.blueprint;
using Fuzzer.core;
using NUnit.Framework;
using Rollback.tests;

namespace Rollback.Tests
{
    public class RollbackRandomFuzzerContext : RollbackFuzzerContext<RollbackRandom>
    {
        public RollbackRandomFuzzerContext(RollbackClock clock) : base(clock, new RollbackRandom(clock, 0), new RollbackRandom(clock, 0))
        {
        }

        public void StepNext(double seed)
        {
            Apply(random => random.Next());
        }

        public void StepNextDouble(double seed)
        {
            Apply(random => random.NextDouble());
        }

        public void StepNextBytes(double seed)
        {
            var arrayLength = FuzzerUtils.SeedToInt(seed, 9) + 1;
            var array = new byte[arrayLength];
            Apply(random => random.NextBytes(array));
        }
    }

    [TestFixture]
    public class RollbackRandomFuzz
    {
        [Test]
        public void Fuzz()
        {
            var blueprint = new FuzzerBlueprint<RollbackRandomFuzzerContext>()
                .Phase(0, 20)
                .Step("StepNext", (context, seed) => context.StepNext(seed))
                .Step("StepNextDouble", (context, seed) => context.StepNextDouble(seed))
                .Step("StepNextBytes", (context, seed) => context.StepNextBytes(seed))
                .Phase(1, 1)
                .Step("StepRollbackPlan", (context, _) => context.StepRollbackPlan())
                .Phase(0, 20)
                .Step("StepNext", (context, seed) => context.StepNext(seed))
                .Step("StepNextDouble", (context, seed) => context.StepNextDouble(seed))
                .Step("StepNextBytes", (context, seed) => context.StepNextBytes(seed))
                .Step("StepRollbackPerform", (context, _) => context.StepRollbackPerform())
                .Phase(1, 1)
                .Step("StepComplete", (context, _) => context.StepComplete());
            var fuzzer = new Fuzzer<RollbackRandomFuzzerContext>(() => new RollbackRandomFuzzerContext(new RollbackClock()));
            fuzzer.Fuzz(blueprint, 500);
        }
    }
}