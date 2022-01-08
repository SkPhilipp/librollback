using System.Collections.Generic;

namespace Rollback
{
    public class RollbackPropertyFrame<T> : RollbackFrame
    {
        public T Value;

        public RollbackPropertyFrame(int time) : base(time)
        {
        }
    }

    public class RollbackProperty<T> : FrameBasedRollback<RollbackPropertyFrame<T>>
    {
        private T _value;

        public RollbackProperty(RollbackClock clock, T value) : base(clock)
        {
            _value = value;
        }

        public T Get()
        {
            return _value;
        }

        public void Set(T value)
        {
            var lastFrame = FrameCurrent();
            if (lastFrame == null || lastFrame.Time != Clock.Time)
            {
                var frame = FrameCreate();
                frame.Value = _value;
                FramePush(frame);
            }

            _value = value;
        }

        protected override RollbackPropertyFrame<T> FrameCreate()
        {
            return new RollbackPropertyFrame<T>(Clock.Time);
        }

        protected override void FrameApply(RollbackPropertyFrame<T> frame)
        {
            _value = frame.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is RollbackProperty<T> objCast && EqualityComparer<T>.Default.Equals(_value, objCast._value);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return EqualityComparer<T>.Default.GetHashCode(_value);
        }

        public override string ToString()
        {
            return $"{_value}";
        }
    }
}