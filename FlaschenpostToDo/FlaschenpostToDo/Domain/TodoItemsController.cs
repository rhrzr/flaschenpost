using FlaschenpostToDo.Domain.Entities;
using FlaschenpostToDo.Network;

namespace FlaschenpostToDo.Domain;

public class TodoItemsController(
    IApiClient apiClient,
    INetworkStatusController networkStatusController) : ITodoController
{
    private readonly HashSet<TodoItem> _pendingToUpload = new();
    private readonly HashSet<TodoItem> _pendingToDelete = new();
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(5);

    public async Task SetTodoCompleteAsync(TodoItem todoItem)
    {
        todoItem.CompletedDateTime = DateTime.Now;
        Todos.OpenTodos.Remove(todoItem);
        Todos.CompletedTodos.Add(todoItem);
        if (await CheckNetworkAsync(true))
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(_defaultTimeout);
            await apiClient.TodoItemsPUTAsync(todoItem.Id, todoItem, cts.Token);
        }
        else
        {
            _pendingToUpload.Add(todoItem);
        }
    }

    public async Task SetTodoIncompleteAsync(TodoItem todoItem)
    {
        todoItem.CompletedDateTime = null;
        Todos.CompletedTodos.Remove(todoItem);
        Todos.OpenTodos.Add(todoItem);
        if (await CheckNetworkAsync(true))
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(_defaultTimeout);
            await apiClient.TodoItemsPUTAsync(todoItem.Id, todoItem, cts.Token);
        }
        else
        {
            _pendingToUpload.Add(todoItem);
        }
    }

    public async Task DeleteTodoAsync(TodoItem todoItem)
    {
        Todos.CompletedTodos.Remove(todoItem);
        _pendingToUpload.Remove(todoItem);
        if (await CheckNetworkAsync(true))
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(_defaultTimeout);
            await apiClient.TodoItemsDELETEAsync(todoItem.Id, cts.Token);
        }
        else
        {
            _pendingToDelete.Add(todoItem);
        }
    }

    public async Task SynchronizeAsync()
    {
        if (!await CheckNetworkAsync())
            return;

        await SendPendingToUploadAsync();
        await SendPendingToDeleteAsync();
        var todoItemComparer = new TodoItemComparer();

        // load open todos
        var cts = new CancellationTokenSource();
        cts.CancelAfter(_defaultTimeout);
        var openTodos = (await LoadOpenTodosAsync(cts.Token)).ToList();
        cts.Dispose();
        openTodos.ToList().ForEach(item =>
        {
            if (Todos.OpenTodos.Contains(item, todoItemComparer))
            {
                openTodos.Remove(item);
                return;
            }

            if (Todos.CompletedTodos.Contains(item, todoItemComparer))
                openTodos.Remove(item);
        });

        // load completed todos
        cts = new CancellationTokenSource();
        cts.CancelAfter(_defaultTimeout);
        var completedTodos = (await LoadCompletedTodosAsync(cts.Token)).ToList();
        cts.Dispose();
        completedTodos.ToList().ForEach(item =>
        {
            // happens, when a completed object could not be sent
            if (Todos.CompletedTodos.Contains(item, todoItemComparer))
                completedTodos.Remove(item);
        });

        Todos.OpenTodos.AddRange(openTodos);
        Todos.CompletedTodos.AddRange(completedTodos);
    }

    private async Task SendPendingToUploadAsync()
    {
        foreach (var item in _pendingToUpload.ToList())
        {
            if (!await CheckNetworkAsync())
                continue;

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(_defaultTimeout);
            await apiClient.TodoItemsPUTAsync(item.Id, item, cts.Token);
            _pendingToUpload.Remove(item);
        }
    }

    private async Task SendPendingToDeleteAsync()
    {
        foreach (var item in _pendingToDelete.ToList())
        {
            if (!await CheckNetworkAsync())
                continue;

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(_defaultTimeout);
            await apiClient.TodoItemsDELETEAsync(item.Id, cts.Token);
            _pendingToDelete.Remove(item);
        }
    }

    private async Task<bool> CheckNetworkAsync(bool silent = false)
    {
        var page = Application.Current?.Windows[0].Page;

        if (!networkStatusController.IsDeviceOnline())
        {
            if (!silent)
                page?.DisplayAlert("Info", "Device is offline", "OK");

            return false;
        }

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(_defaultTimeout);
        if (!await networkStatusController.IsApiServerReachableAsync(cts.Token))
        {
            if (!silent)
                page?.DisplayAlert("Info", "No connection to the API server", "OK");

            return false;
        }

        return true;
    }

    private async Task<IEnumerable<TodoItem>> LoadOpenTodosAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<TodoItem> openTodos = new List<TodoItem>();
        try
        {
            openTodos = await apiClient.OpenAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _ = Application.Current?.Windows[0].Page?.DisplayAlert("Warning", "No connection to the API server", "OK");
        }

        return openTodos;
    }

    private async Task<IEnumerable<TodoItem>> LoadCompletedTodosAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<TodoItem> completedTodos = new List<TodoItem>();
        try
        {
            completedTodos = await apiClient.CompletedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _ = Application.Current?.Windows[0].Page?.DisplayAlert("Warning", "No connection to the API server", "OK");
        }

        return completedTodos;
    }

    public Todos Todos { get; } = new([], []);
}