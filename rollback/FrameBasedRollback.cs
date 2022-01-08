namespace Rollback
{
    /// <summary>
    /// Rollback information for a single frame of changes on an object.
    /// </summary>
    public abstract class RollbackFrame
    {
        protected RollbackFrame(int time)
        {
            Time = time;
        }

        public int Time { get; }
    }

    /// <summary>
    /// Frame-based rollback implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FrameBasedRollback<T> : IRollback where T : RollbackFrame
    {
        private readonly T[] _frames;
        private int _frameIndex;

        protected FrameBasedRollback()
        {
            _frames = new T[RollbackConfiguration.Frames];
            _frameIndex = 0;
        }

        protected abstract T FrameCreate(int frameTime);

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
        /// <param name="frameTime"></param>
        /// <returns></returns>
        protected T Frame(int frameTime)
        {
            var lastFrame = _frames[_frameIndex];
            if (lastFrame != null && lastFrame.Time == frameTime)
            {
                return lastFrame;
            }

            var newFrame = FrameCreate(frameTime);
            FramePush(newFrame);
            return newFrame;
        }

        protected abstract void FrameApply(T frame);

        public void Rollback(int frameTime)
        {
            const int limit = RollbackConfiguration.Frames;
            var i = _frameIndex + limit;
            while (i >= _frameIndex)
            {
                var index = i % limit;
                var frame = _frames[index];
                if (frame == null || frame.Time < frameTime)
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