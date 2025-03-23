using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlaschenpostToDo.Domain;
using FlaschenpostToDo.Network;
using FlaschenpostToDo.Presentation.View;

namespace FlaschenpostToDo.Presentation.ViewModel;

public partial class TodoViewModel(
    ITodoController todoController) : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TodoItem> _todoItems = new();

    public override void OnAppearing()
    {
        UpdateItems();
    }

    private void UpdateItems()
    {
        var todos = todoController.Todos;
        TodoItems.Clear();
        todos.OpenTodos.OrderBy(todo => todo.DueDateTime).ToList().ForEach(todoItem => TodoItems.Add(todoItem));
    }

    [RelayCommand]
    private async Task Tap(TodoItem todoItem)
    {
        var parameters = new ShellNavigationQueryParameters
        {
            { "todo", todoItem }
        };
        await Shell.Current.GoToAsync(nameof(DetailsPage), true, parameters);
    }

    [RelayCommand]
    private async Task LeftSwipe(TodoItem todoItem)
    {
        TodoItems.Remove(todoItem);
        await todoController.SetTodoCompleteAsync(todoItem);
    }

    [RelayCommand]
    private async Task Synchronize()
    {
        await todoController.SynchronizeAsync();
        UpdateItems();
    }
}