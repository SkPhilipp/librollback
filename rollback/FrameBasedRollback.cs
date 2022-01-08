namespace Rollback
{
    /// <summary>
    /// Rollback information for a single frame of changes on an object.
    /// </summary>
    public abstract class RollbackFrame
    {
        public readonly int Time;

        protected RollbackFrame(int time)
        {
            Time = time;
        }
    }

    /// <summary>
    /// Rollback implementation where each change is registered onto a frame containing all changes made for the current time.
    ///
    /// Frames generally contain only the minimal amount of information to be able to undo the changes. For example when modifying a single value multiple times within
    /// a single frame, only the original value needs to be stored to perform a rollback.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FrameBasedRollback<T> : IRollback where T : RollbackFrame
    {
        protected readonly RollbackClock Clock;
        private readonly T[] _frames;
        private int _frameIndex;

        protected FrameBasedRollback(RollbackClock clock)
        {
            Clock = clock;
            _frames = new T[RollbackConfiguration.Frames];
            _frameIndex = 0;
        }

        /// <summary>
        /// Initialize an empty frame at the current clock time.
        /// </summary>
        /// <returns></returns>
        protected abstract T FrameCreate();

        protected T FrameCurrent()
        {
            return _frames[_frameIndex];
        }

        protected void FramePush(T frame)
        {
            _frameIndex = (_frameIndex + 1) % RollbackConfiguration.Frames;
            _frames[_frameIndex] = frame;
        }

        /// <summary>
        /// Retrieves or creates the frame for the given time.
        /// </summary>
        /// <returns></returns>
        protected T Frame()
        {
            var lastFrame = _frames[_frameIndex];
            if (lastFrame != null && lastFrame.Time == Clock.Time)
            {
                return lastFrame;
            }

            var newFrame = FrameCreate();
            FramePush(newFrame);
            return newFrame;
        }

        /// <summary>
        /// Apply a frame of changes to this object, invoked by <see cref="Rollback"/>.
        /// </summary>
        /// <param name="frame"></param>
        protected abstract void FrameApply(T frame);

        public void Rollback()
        {
            const int limit = RollbackConfiguration.Frames;
            var i = _frameIndex + limit;
            while (i >= _frameIndex)
            {
                var index = i % limit;
                var frame = _frames[index];
                if (frame == null || frame.Time < Clock.Time)
                {
                    break;
                }

                FrameApply(frame);
                _frames[index] = null;
                i--;
                _frameIndex = (i - 1) % limit;
            }
        }

        /// <summary>
        /// Resets rollback data.
        /// </summary>
        public void RollbackClear()
        {
            _frames[_frameIndex] = null;
        }
    }
}