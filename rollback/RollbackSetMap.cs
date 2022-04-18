using System.Collections.Generic;

namespace Rollback
{
    /// <summary>
    /// Entry within a RollbackSetMapFrame, can only be compared with the same type.
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class RollbackSetMapFrameEntry<TK, TV>
    {
        public readonly TK Key;
        public readonly TV Value;
        public readonly bool WasAssigned;

        public RollbackSetMapFrameEntry(TK key, TV value, bool wasAssigned)
        {
            Key = key;
            Value = value;
            WasAssigned = wasAssigned;
        }

        /// <summary>
        /// Determines whether the specified RollbackSetMapFrameEntry is equal to the current object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as RollbackSetMapFrameEntry<TK, TV>;
            // ReSharper disable once PossibleNullReferenceException
            return EqualityComparer<TK>.Default.Equals(Key, other.Key) && EqualityComparer<TV>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return (EqualityComparer<TK>.Default.GetHashCode(Key) * 397) ^ EqualityComparer<TV>.Default.GetHashCode(Value);
        }
    }

    public class RollbackSetMapFrame<TK, TV> : RollbackFrame
    {
        public readonly HashSet<RollbackSetMapFrameEntry<TK, TV>> OriginalValues;

        public RollbackSetMapFrame(int time) : base(time)
        {
            OriginalValues = new HashSet<RollbackSetMapFrameEntry<TK, TV>>();
        }

        public void RegisterUnassigned(TK key, TV value)
        {
            var entry = new RollbackSetMapFrameEntry<TK, TV>(key, value, false);
            if (!OriginalValues.Contains(entry))
            {
                OriginalValues.Add(entry);
            }
        }

        public void RegisterAssigned(TK key, TV value)
        {
            var entry = new RollbackSetMapFrameEntry<TK, TV>(key, value, true);
            if (!OriginalValues.Contains(entry))
            {
                OriginalValues.Add(entry);
            }
        }
    }

    /// <summary>
    /// A map of sets; Sorted sets of TV grouped by instances of TK.
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class RollbackSetMap<TK, TV> : FrameBasedRollback<RollbackSetMapFrame<TK, TV>>, IRollbackSetMapReadOnly<TK, TV>
    {
        private readonly Dictionary<TK, SortedSet<TV>> _values;

        public RollbackSetMap(RollbackClock clock) : base(clock)
        {
            _values = new Dictionary<TK, SortedSet<TV>>();
        }

        public void Add(TK key, TV value)
        {
            _values.TryGetValue(key, out var entry);
            if (entry == null)
            {
                var frame = Frame();
                frame.RegisterUnassigned(key, value);
                entry = new SortedSet<TV>();
                entry.Add(value);
                _values[key] = entry;
            }
            else if (!entry.Contains(value))
            {
                var frame = Frame();
                frame.RegisterUnassigned(key, value);
                entry.Add(value);
            }
        }

        public void Remove(TK key, TV value)
        {
            _values.TryGetValue(key, out var entry);
            if (entry != null && entry.Remove(value))
            {
                var frame = Frame();
                frame.RegisterAssigned(key, value);
                if (entry.Count == 0)
                {
                    _values.Remove(key);
                }
            }
        }

        public void RemoveAll(TK key)
        {
            _values.TryGetValue(key, out var entry);
            if (entry != null)
            {
                var frame = Frame();
                foreach (var value in entry)
                {
                    frame.RegisterAssigned(key, value);
                }

                _values.Remove(key);
            }
        }

        public bool ContainsKey(TK key)
        {
            return _values.ContainsKey(key);
        }

        public SortedSet<TK> Keys()
        {
            return new SortedSet<TK>(_values.Keys);
        }

        public IReadOnlyCollection<TV> this[TK key] => _values[key];

        public IReadOnlyCollection<TV> TryGetValue(TK key)
        {
            _values.TryGetValue(key, out var value);
            return value;
        }

        protected override RollbackSetMapFrame<TK, TV> FrameCreate()
        {
            return new RollbackSetMapFrame<TK, TV>(Clock.Time);
        }

        protected override void FrameApply(RollbackSetMapFrame<TK, TV> frame)
        {
            foreach (var frameEntry in frame.OriginalValues)
            {
                _values.TryGetValue(frameEntry.Key, out var entry);
                if (frameEntry.WasAssigned)
                {
                    if (entry != null)
                    {
                        entry.Add(frameEntry.Value);
                    }
                    else
                    {
                        entry = new SortedSet<TV>();
                        entry.Add(frameEntry.Value);
                        _values[frameEntry.Key] = entry;
                    }
                }
                else
                {
                    if (entry != null)
                    {
                        entry.Remove(frameEntry.Value);
                        if (entry.Count == 0)
                        {
                            _values.Remove(frameEntry.Key);
                        }
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is RollbackSetMap<TK, TV> other)
            {
                if (Keys().SetEquals(other.Keys()))
                {
                    foreach (var key in Keys())
                    {
                        if (!_values[key].SetEquals(other[key]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
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