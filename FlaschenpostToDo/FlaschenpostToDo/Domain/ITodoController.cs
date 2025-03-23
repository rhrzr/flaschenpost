using FlaschenpostToDo.Domain.Entities;
using FlaschenpostToDo.Network;

namespace FlaschenpostToDo.Domain;

public interface ITodoController
{
    public Task SetTodoCompleteAsync(TodoItem todoItem);
    public Task SetTodoIncompleteAsync(TodoItem todoItem);
    public Task DeleteTodoAsync(TodoItem todoItem);
    public Task SynchronizeAsync();

    public Todos Todos { get; }
}