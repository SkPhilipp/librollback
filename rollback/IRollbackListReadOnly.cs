using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rollback
{
    public interface IRollbackListReadOnly<T>
    {
        ReadOnlyCollection<T> Values();

        List<T>.Enumerator GetEnumerator();

        int Count { get; }

        T this[int index] { get; }
    }
}