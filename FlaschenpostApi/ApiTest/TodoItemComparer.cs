using FlaschenpostApi.Entities;

namespace ApiTest;

public class TodoItemComparer : IEqualityComparer<TodoItem>
{
    public bool Equals(TodoItem? x, TodoItem? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        // ignore id
        return string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase)
               && Nullable.Equals(x.IssuedDateTime, y.IssuedDateTime)
               && Nullable.Equals(x.DueDateTime, y.DueDateTime)
               && Nullable.Equals(x.LastUpdatedDateTime, y.LastUpdatedDateTime)
               && Nullable.Equals(x.CompletedDateTime, y.CompletedDateTime)
               && string.Equals(x.Description, y.Description, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(TodoItem obj)
    {
        var hashCode = new HashCode();
        // ignore id
        hashCode.Add(obj.Title, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(obj.IssuedDateTime);
        hashCode.Add(obj.DueDateTime);
        hashCode.Add(obj.LastUpdatedDateTime);
        hashCode.Add(obj.CompletedDateTime);
        hashCode.Add(obj.Description, StringComparer.OrdinalIgnoreCase);
        return hashCode.ToHashCode();
    }
}