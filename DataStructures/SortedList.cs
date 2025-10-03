
namespace DataStructures;

/// <summary>
///     Implementation of SortedList using binary search.
/// </summary>
/// <typeparam name="T">Generic Type.</typeparam>
public class SortedList<T> : IEnumerable<T>
{
    private readonly IComparer<T> comparer;
    private readonly List<T> memory = new();

    public SortedList() : this(Comparer<T>.Default) { }
    public SortedList(IComparer<T> comparer) => this.comparer = comparer;

    public int Count => memory.Count;
    public void Add(T item) => memory.Insert(IndexFor(item, out _), item);
    public T this[int i] => memory[i];
    public void Clear() => memory.Clear();
    /// <param name="item">An element to search.</param>
    /// <returns>true - <see cref="SortedList{T}" /> contains an element, otherwise - false.</returns>
    public bool Contains(T item)
    {
        _ = IndexFor(item, out var found);
        return found;
    }

    /// <summary>
    /// Removes a certain element from <see cref="SortedList{T}" />.
    /// </summary>
    /// <param name="item">An element to remove.</param>
    /// <returns>true - element is found and removed, otherwise false.</returns>
    public bool TryRemove(T item)
    {
        var index = IndexFor(item, out var found);

        if (found)
        {
            memory.RemoveAt(index);
        }

        return found;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="SortedList{T}" />.
    /// </summary>
    /// <returns>A Enumerator for the <see cref="SortedList{T}" />.</returns>
    public IEnumerator<T> GetEnumerator()
        => memory.GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Binary search algorithm for finding element index in <see cref="SortedList{T}" />.
    /// </summary>
    /// <param name="item">Element.</param>
    /// <param name="found">Indicates whether the equal value was found in <see cref="SortedList{T}" />.</param>
    /// <returns>Index for the Element.</returns>
    private int IndexFor(T item, out bool found)
    {
        var left = 0;
        var right = memory.Count;

        while (right - left > 0)
        {
            var mid = (left + right) / 2;

            switch (comparer.Compare(item, memory[mid]))
            {
                case > 0:
                    left = mid + 1;
                    break;
                case < 0:
                    right = mid;
                    break;
                default:
                    found = true;
                    return mid;
            }
        }

        found = false;
        return left;
    }
}
