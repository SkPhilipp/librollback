namespace Rollback.Tests
{
    public class EmbeddedObjectExample : IRollback
    {
        private readonly RollbackClock _clock;
        private readonly RollbackProperty<int> _x;
        private readonly RollbackProperty<int> _y;

        public EmbeddedObjectExample(RollbackClock clock)
        {
            _clock = clock;
            _x = new RollbackProperty<int>(clock, 0);
            _y = new RollbackProperty<int>(clock, 0);
        }

        public int X
        {
            get => _x.Get();
            set => _x.Set(value);
        }

        public int Y
        {
            get => _y.Get();
            set => _y.Set(value);
        }

        public void Rollback()
        {
            _x.Rollback();
            _y.Rollback();
        }

        public void RollbackClear()
        {
            _x.RollbackClear();
            _y.RollbackClear();
        }
    }
}