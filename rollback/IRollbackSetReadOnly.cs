using System.Collections.Generic;

namespace Rollback
{
    public interface IRollbackSetReadOnly<T>
    {
        bool Has(T value);

        SortedSet<T>.Enumerator GetEnumerator();

        int Count { get; }
    }
}