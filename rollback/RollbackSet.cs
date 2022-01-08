using System.Collections.Generic;

namespace Rollback
{
    public class RollbackSetFrame<T> : RollbackFrame
    {
        public readonly Dictionary<T, bool> OriginalValues;

        public RollbackSetFrame(int time) : base(time)
        {
            OriginalValues = new Dictionary<T, bool>();
        }

        public bool IsUnregistered(T key)
        {
            return !OriginalValues.ContainsKey(key);
        }

        public void RegisterOriginal(T key)
        {
            OriginalValues.Add(key, true);
        }

        public void RegisterUnassigned(T key)
        {
            OriginalValues.Add(key, false);
        }
    }

    public class RollbackSet<T> : FrameBasedRollback<RollbackSetFrame<T>>
    {
        private readonly HashSet<T> _values;

        public RollbackSet(RollbackClock clock) : base(clock)
        {
            _values = new HashSet<T>();
        }

        public RollbackSet(RollbackClock clock, IEnumerable<T> initialValues) : this(clock)
        {
            foreach (var initialValue in initialValues)
            {
                _values.Add(initialValue);
            }
        }

        public bool Has(T value)
        {
            return _values.Contains(value);
        }

        public void Add(T value)
        {
            if (!_values.Contains(value))
            {
                var frame = Frame();
                if (frame.IsUnregistered(value))
                {
                    frame.RegisterUnassigned(value);
                }

                _values.Add(value);
            }
        }

        public void Remove(T value)
        {
            if (Has(value))
            {
                var frame = Frame();
                if (frame.IsUnregistered(value))
                {
                    frame.RegisterOriginal(value);
                }

                _values.Remove(value);
            }
        }

        /// <summary>
        /// Enumerates a sorted view on this set's internal values. Sorting is applied to preserve the order of values
        /// between rollbacks, as the internal values set may go out of order when applying changes to it and rolling
        /// them back.
        /// </summary>
        /// <returns></returns>
        public SortedSet<T>.Enumerator GetEnumerator()
        {
            return new SortedSet<T>(_values).GetEnumerator();
        }

        public int Count => _values.Count;

        protected override RollbackSetFrame<T> FrameCreate()
        {
            return new RollbackSetFrame<T>(Clock.Time);
        }

        protected override void FrameApply(RollbackSetFrame<T> frame)
        {
            foreach (var pair in frame.OriginalValues)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (value)
                {
                    _values.Add(key);
                }
                else
                {
                    _values.Remove(key);
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is RollbackSet<T> objCast && _values.SetEquals(objCast._values);
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