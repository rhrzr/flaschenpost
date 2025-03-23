using FlaschenpostToDo.Network;

namespace FlaschenpostToDo.Domain.Entities;

public record Todos(
    List<TodoItem> OpenTodos,
    List<TodoItem> CompletedTodos);