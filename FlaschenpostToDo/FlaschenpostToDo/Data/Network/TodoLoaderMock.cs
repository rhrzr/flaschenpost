using FlaschenpostToDo.Network;

namespace FlaschenpostToDo.Data.Network;

public class TodoLoaderMock
{
    private const int countMockTasks = 10;
    private readonly Random _random = new();
    private readonly List<TodoItem> _generatedTasks;

    private readonly List<string> _taskNames =
    [
        "Apples", "Bananas", "Oranges", "Pineapples", "Cherries", "Mangos", "Melons", "Apricots", "Raspberries", "Peaches",
        "Chocolate Bars", "Chips", "Cornflakes", "Hamburgers", "Cakes", "Candies"
    ];

    public TodoLoaderMock()
    {
        var items = new List<TodoItem>();
        for (var i = 0; i < countMockTasks; i++)
        {
            var task = GenerateTask();
            items.Add(task);
        }

        _generatedTasks = items;
    }

    public Task<List<TodoItem>> LoadAllTodosAsync()
    {
        return Task.Run(() => _generatedTasks);
    }

    private TodoItem GenerateTask()
    {
        const int dueDaysRange = 5;
        const int dueHoursRange = 8;
        var title = _taskNames[_random.Next(_taskNames.Count)];
        var task = new TodoItem
        {
            Id = _random.Next(),
            Title = title,
            // Description = DescriptionFunc(title),
            IssuedDateTime = DateTime.Now,
            DueDateTime = DateTime.Now
                .AddDays(_random.Next(0, dueDaysRange + 1))
                .AddHours(_random.Next(1, dueHoursRange + 1))
        };

        return task;

        string DescriptionFunc(string text) => $"You should eat more {text}";
    }
}