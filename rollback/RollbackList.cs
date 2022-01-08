using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rollback
{
    public class RollbackListFrame<T> : RollbackFrame
    {
        /// <summary>
        /// Original length that the target list would be set back to when this rollback frame executes.
        /// </summary>
        public int OriginalLength { get; }

        /// <summary>
        /// Previous values for elements which were either removed or assigned new values within the span of this frame.
        /// </summary>
        public Dictionary<int, T> OriginalValues { get; }

        public RollbackListFrame(int time, int originalLength) : base(time)
        {
            OriginalLength = originalLength;
            OriginalValues = new Dictionary<int, T>();
        }

        /// <summary>
        /// Registers a previous value of the target list, to be invoked when the target list's elements change.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="previousValue"></param>
        public void RegisterPrevious(int index, T previousValue)
        {
            if (index < OriginalLength && !OriginalValues.ContainsKey(index))
            {
                OriginalValues.Add(index, previousValue);
            }
        }
    }

    public class RollbackList<T> : FrameBasedRollback<RollbackListFrame<T>>
    {
        private readonly List<T> _values;

        public RollbackList(RollbackClock clock, int capacity = 10) : base(clock)
        {
            _values = new List<T>(capacity);
        }

        /// <summary>
        /// Retrieves the underlying list.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<T> Values()
        {
            return _values.AsReadOnly();
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public int Count => _values.Count;

        public void Push(IEnumerable<T> values)
        {
            // notes the original length by ensuring a frame exists
            Frame();
            _values.AddRange(values);
        }

        public T Pop()
        {
            var length = _values.Count;
            if (length == 0)
            {
                return default;
            }

            var frame = Frame();
            var value = _values[length - 1];
            frame.RegisterPrevious(length - 1, value);
            _values.RemoveAt(length - 1);
            return value;
        }

        public void Set(int index, T value)
        {
            var frame = Frame();
            if (index < _values.Count)
            {
                frame.RegisterPrevious(index, _values[index]);
            }
            else
            {
                _values.AddRange(new T[index - _values.Count + 1]);
            }

            _values[index] = value;
        }

        public T this[int index]
        {
            get => _values[index];
            set => Set(index, value);
        }

        protected override RollbackListFrame<T> FrameCreate()
        {
            return new RollbackListFrame<T>(Clock.Time, _values.Count);
        }

        protected override void FrameApply(RollbackListFrame<T> frame)
        {
            if (_values.Count > frame.OriginalLength)
            {
                _values.RemoveRange(frame.OriginalLength, _values.Count - frame.OriginalLength);
            }

            if (_values.Count < frame.OriginalLength)
            {
                _values.AddRange(new T[frame.OriginalLength - _values.Count]);
            }

            foreach (var pair in frame.OriginalValues)
            {
                _values[pair.Key] = pair.Value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is RollbackList<T> objCast && _values.SequenceEqual(objCast._values);
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(", ", _values);
        }
    }
}