using System.Collections.Generic;

namespace Rollback
{
    public interface IRollbackMapReadOnly<TK, out TV>
    {
        bool ContainsKey(TK key);

        SortedSet<TK> Keys();

        TV this[TK key] { get; }
    }
}