using System;

namespace Rollback
{
    public class RollbackRandomFrame : RollbackFrame
    {
        public readonly int Seed;

        public RollbackRandomFrame(int time, int seed) : base(time)
        {
            Seed = seed;
        }
    }

    public class RollbackRandom : FrameBasedRollback<RollbackRandomFrame>
    {
        private Random _random;
        private int _seed;

        public RollbackRandom(RollbackClock clock, int seed) : base(clock)
        {
            _seed = seed;
            _random = new Random(_seed);
        }

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public int Next()
        {
            return _random.Next();
        }

        public void NextBytes(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }

        protected override RollbackRandomFrame FrameCreate()
        {
            _seed = _random.Next();
            return new RollbackRandomFrame(Clock.Time, _seed);
        }

        protected override void FrameApply(RollbackRandomFrame frame)
        {
            _seed = frame.Seed;
            _random = new Random(_seed);
        }

        private bool Equals(RollbackRandom other)
        {
            return _seed == other._seed;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(obj as RollbackRandom);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}