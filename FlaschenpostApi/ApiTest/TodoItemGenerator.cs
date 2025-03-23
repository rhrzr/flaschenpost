using System.Collections;
using FlaschenpostApi.Entities;

namespace ApiTest;

public class TodoItemGenerator : IEnumerable<object[]>
{
    private readonly Random _random = new();

    public IEnumerator<object[]> GetEnumerator()
    {
        var items = new List<object[]>();
        var randomTodos = new List<TodoItem>();
        const int minItems = 3;
        const int maxItems = 6;
        for (var i = 0; i < _random.Next(minItems, maxItems + 1); i++)
        {
            randomTodos.Add(GenerateItem());
        }

        items.Add([randomTodos]);
        return items.GetEnumerator();
    }

    private TodoItem GenerateItem()
    {
        var guid = Guid.NewGuid().ToString();
        const int minHours = -8;
        const int maxHours = 0;
        const int minDays = 1;
        const int maxDays = 5;
        var item = new TodoItem
        {
            Title = guid,
            IssuedDateTime = DateTime.Now.AddHours(_random.Next(minHours, maxHours + 1)),
            DueDateTime = DateTime.Now.AddDays(_random.Next(minDays, maxDays + 1)),
            Description = $"Description for #{guid}"
        };

        return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}