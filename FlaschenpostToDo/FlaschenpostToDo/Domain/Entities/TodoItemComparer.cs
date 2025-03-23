using FlaschenpostToDo.Network;

namespace FlaschenpostToDo.Domain.Entities;

public class TodoItemComparer : IEqualityComparer<TodoItem>
{
    public bool Equals(TodoItem? x, TodoItem? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id
               && string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase)
               && x.IssuedDateTime.Equals(y.IssuedDateTime)
               && x.DueDateTime.Equals(y.DueDateTime)
               && Nullable.Equals(x.LastUpdatedDateTime, y.LastUpdatedDateTime)
               && Nullable.Equals(x.CompletedDateTime, y.CompletedDateTime)
               && string.Equals(x.Description, y.Description, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(TodoItem obj)
    {
        var hashCode = new HashCode();
        hashCode.Add(obj.Id);
        hashCode.Add(obj.Title, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(obj.IssuedDateTime);
        hashCode.Add(obj.DueDateTime);
        hashCode.Add(obj.LastUpdatedDateTime);
        hashCode.Add(obj.CompletedDateTime);
        hashCode.Add(obj.Description, StringComparer.OrdinalIgnoreCase);
        return hashCode.ToHashCode();
    }
}