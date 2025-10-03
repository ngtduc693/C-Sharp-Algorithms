namespace DataStructures.Stack;

/// <summary>
///     Implementation of a list based stack. FILO style.
/// </summary>
/// <typeparam name="T">Generic Type.</typeparam>
public class ListBasedStack<T>
{
    // LinkedList-based stack.
    private readonly LinkedList<T> stack = new();

    public ListBasedStack() { }
    public ListBasedStack(T item) : this() => Push(item);
    public ListBasedStack(IEnumerable<T> items) : this() { foreach (var item in items) Push(item); }

    public int Count => stack.Count;
    public void Clear() => stack.Clear();
    public bool Contains(T item) => stack.Contains(item);

    public T Peek() => stack.First?.Value ?? throw new InvalidOperationException("Stack is empty");

    public T Pop()
    {
        if (stack.First is null) throw new InvalidOperationException("Stack is empty");
        var item = stack.First.Value;
        stack.RemoveFirst();
        return item;
    }

    public void Push(T item) => stack.AddFirst(item);
}
