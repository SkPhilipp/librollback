using Rollback.structures;

namespace Rollback.Tests.structures
{
    public class EmbeddedObjectExample : IRollbackEmbedded
    {
        private readonly RollbackClock _clock;
        private readonly RollbackProperty<int> _x;
        private readonly RollbackProperty<int> _y;

        public EmbeddedObjectExample(RollbackClock clock)
        {
            _clock = clock;
            _x = new RollbackProperty<int>(0);
            _y = new RollbackProperty<int>(0);
        }

        public int X
        {
            get => _x.Get();
            set => _x.Set(_clock.Time, value);
        }

        public int Y
        {
            get => _y.Get();
            set => _y.Set(_clock.Time, value);
        }

        public void Rollback()
        {
            _x.Rollback(_clock.Time);
            _y.Rollback(_clock.Time);
        }

        public void RollbackClear()
        {
            _x.RollbackClear();
            _y.RollbackClear();
        }
    }
}