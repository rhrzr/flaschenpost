using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlaschenpostToDo.Domain;
using FlaschenpostToDo.Network;
using FlaschenpostToDo.Presentation.View;

namespace FlaschenpostToDo.Presentation.ViewModel;

public partial class DoneViewModel(ITodoController todoController) : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TodoItem> _doneItems = new();

    public override void OnAppearing()
    {
        UpdateItems();
    }

    private void UpdateItems()
    {
        var todos = todoController.Todos;
        DoneItems.Clear();
        todos.CompletedTodos.OrderByDescending(todo => todo.CompletedDateTime).ToList().ForEach(todoItem => DoneItems.Add(todoItem));
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
    private async Task RightSwipe(TodoItem todoItem)
    {
        DoneItems.Remove(todoItem);
        await todoController.SetTodoIncompleteAsync(todoItem);
    }
    
    [RelayCommand]
    private async Task LeftSwipe(TodoItem todoItem)
    {
        DoneItems.Remove(todoItem);
        await todoController.DeleteTodoAsync(todoItem);
    }
}