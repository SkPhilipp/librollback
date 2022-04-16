using System.Collections.Generic;

namespace Rollback
{
    public interface IRollbackSetMapReadOnly<TK, out TV>
    {
        bool ContainsKey(TK key);

        SortedSet<TK> Keys();

        IReadOnlyCollection<TV> this[TK key] { get; }
    }
}