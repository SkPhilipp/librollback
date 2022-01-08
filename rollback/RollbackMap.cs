using System.Collections.Generic;

namespace Rollback
{
    public class RollbackMapFrameValue<T>
    {
        public T Original;
        public bool WasAssigned;
    }

    public class RollbackMapFrame<TK, TV> : RollbackFrame
    {
        public readonly Dictionary<TK, RollbackMapFrameValue<TV>> OriginalValues;

        public RollbackMapFrame(int time) : base(time)
        {
            OriginalValues = new Dictionary<TK, RollbackMapFrameValue<TV>>();
        }

        public bool IsUnregistered(TK key)
        {
            return !OriginalValues.ContainsKey(key);
        }

        public void RegisterOriginal(TK key, TV value)
        {
            var frameValue = new RollbackMapFrameValue<TV>
            {
                Original = value,
                WasAssigned = true
            };
            OriginalValues.Add(key, frameValue);
        }

        public void RegisterUnassigned(TK key)
        {
            var frameValue = new RollbackMapFrameValue<TV>
            {
                WasAssigned = false
            };
            OriginalValues.Add(key, frameValue);
        }
    }

    public class RollbackMap<TK, TV> : FrameBasedRollback<RollbackMapFrame<TK, TV>>
    {
        private readonly Dictionary<TK, TV> _values;

        public RollbackMap(RollbackClock clock) : base(clock)
        {
            _values = new Dictionary<TK, TV>();
        }

        public bool ContainsKey(TK key)
        {
            return _values.ContainsKey(key);
        }

        public void Set(TK key, TV value)
        {
            var frame = Frame();
            if (frame.IsUnregistered(key))
            {
                var wasAssigned = _values.ContainsKey(key);
                if (wasAssigned)
                {
                    frame.RegisterOriginal(key, _values[key]);
                }
                else
                {
                    frame.RegisterUnassigned(key);
                }
            }

            _values[key] = value;
        }

        public void Remove(TK key)
        {
            if (_values.ContainsKey(key))
            {
                var frame = Frame();
                if (frame.IsUnregistered(key))
                {
                    var previous = _values[key];
                    frame.RegisterOriginal(key, previous);
                }

                _values.Remove(key);
            }
        }

        /// <summary>
        /// Enumerates a sorted view on this map's internal values. Sorting is applied to preserve the order of values
        /// between rollbacks, as the internal values set may go out of order when applying changes to it and rolling
        /// them back.
        /// </summary>
        /// <returns></returns>
        public SortedSet<TK> Keys()
        {
            return new SortedSet<TK>(_values.Keys);
        }
        
        public TV this[TK key]
        {
            get => _values[key];
            set => Set(key, value);
        }

        protected override RollbackMapFrame<TK, TV> FrameCreate()
        {
            return new RollbackMapFrame<TK, TV>(Clock.Time);
        }

        protected override void FrameApply(RollbackMapFrame<TK, TV> frame)
        {
            foreach (var pair in frame.OriginalValues)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (value.WasAssigned)
                {
                    _values[key] = value.Original;
                }
                else
                {
                    _values.Remove(key);
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is RollbackMap<TK, TV> objCast && Keys().SetEquals(objCast.Keys());
        }

        public override int GetHashCode()
        {
            return _values.Count.GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(", ", _values);
        }
    }
}