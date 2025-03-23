using System.Net;
using System.Net.Http.Json;
using System.Text;
using FlaschenpostApi;
using FlaschenpostApi.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace ApiTest;

public class TodoTests
{
    [Fact]
    public async Task TestGetAllEmpty()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync("/todoItems");
        var content = await getResponse.Content.ReadFromJsonAsync<List<TodoItem>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(content);
        Assert.Empty(content);
    }

    [Theory]
    [ClassData(typeof(TodoItemGenerator))]
    public async Task TestPostValidData(List<TodoItem> testObjects)
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var serializedTestObjects = JsonConvert.SerializeObject(testObjects);
        var postResponse = await client.PostAsync("/todoItems", new StringContent(serializedTestObjects, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);

        await GetAllAndDelete();
    }

    [Theory]
    [ClassData(typeof(TodoItemGenerator))]
    public async Task TestPostAndGet(List<TodoItem> testObjects)
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // post data
        var serializedTestObjects = JsonConvert.SerializeObject(testObjects);
        var postResponse = await client.PostAsync("/todoItems", new StringContent(serializedTestObjects, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);

        // get all
        var getResponse = await client.GetAsync("/todoItems");
        var content = await getResponse.Content.ReadFromJsonAsync<List<TodoItem>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(content);
        Assert.NotEmpty(content);
        var orderedContent = content.OrderBy(item => item.Id).ToList();
        var comparer = new TodoItemComparer();

        // assert each uri exists
        await Parallel.ForEachAsync(orderedContent, async (item, token) =>
        {
            var currentResponse = await client.GetAsync($"/todoItems/{item.Id}", token);
            var currentContent = await currentResponse.Content.ReadFromJsonAsync<TodoItem>(cancellationToken: token);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(currentContent);
            Assert.Equal(orderedContent.First(old => old.Id == item.Id), currentContent, comparer);
        });

        await GetAllAndDelete();
    }

    [Fact]
    public async Task TestPostInvalidData()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var postResponse = await client.PostAsync("/todoItems", new StringContent("[{\"invalid\": \"data\"}]", Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);

        postResponse = await client.PostAsync("/todoItems", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
    }

    [Theory]
    [ClassData(typeof(TodoItemGenerator))]
    public async Task TestPostAndGetAll(List<TodoItem> testObjects)
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // post data
        var serializedTestObjects = JsonConvert.SerializeObject(testObjects);
        var postResponse = await client.PostAsync("/todoItems", new StringContent(serializedTestObjects, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);

        // get all
        var getResponse = await client.GetAsync("/todoItems");
        var content = await getResponse.Content.ReadFromJsonAsync<List<TodoItem>>();
        var comparer = new TodoItemComparer();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(content);
        Assert.NotEmpty(content);

        // assert data equals
        var orderedTestObjects = testObjects.OrderBy(item => item.Title);
        var orderedContent = content.OrderBy(item => item.Title);

        Assert.Equal(orderedTestObjects, orderedContent, comparer);

        await GetAllAndDelete();
    }

    [Theory]
    [ClassData(typeof(TodoItemGenerator))]
    public async Task TestPutData(List<TodoItem> testObjects)
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // post data
        var serializedTestObjects = JsonConvert.SerializeObject(testObjects);
        var postResponse = await client.PostAsync("/todoItems", new StringContent(serializedTestObjects, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);

        // get all
        var getResponse = await client.GetAsync("/todoItems");
        var content = await getResponse.Content.ReadFromJsonAsync<List<TodoItem>>();

        Assert.NotNull(content);
        Assert.NotEmpty(content);

        // update and put data
        await Parallel.ForEachAsync(content, async (item, token) =>
        {
            item.CompletedDateTime = DateTime.Now;
            var serializedUpdatedData = JsonConvert.SerializeObject(item);
            var putResponse = await client.PutAsync($"/todoItems/{item.Id}", new StringContent(serializedUpdatedData, Encoding.UTF8, "application/json"), token);
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        });

        await GetAllAndDelete();
    }

    [Fact]
    public async Task TestPutNotExisting()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var todoItem = new TodoItem
        {
            Id = 1,
            Title = "test",
            IssuedDateTime = DateTime.Now,
            DueDateTime = DateTime.Now,
            CompletedDateTime = DateTime.Now,
            Description = "test description"
        };
        var content = JsonConvert.SerializeObject(todoItem);
        var putResponse = await client.PutAsync($"/todoItems/{todoItem.Id}", new StringContent(content, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
        await TestGetAllEmpty();
    }

    [Theory]
    [ClassData(typeof(TodoItemGenerator))]
    public async Task TestPostAndDelete(List<TodoItem> testObjects)
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var serializedTestObjects = JsonConvert.SerializeObject(testObjects);
        var postResponse = await client.PostAsync("/todoItems", new StringContent(serializedTestObjects, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);

        // get all
        var getResponse = await client.GetAsync("/todoItems");
        var content = await getResponse.Content.ReadFromJsonAsync<List<TodoItem>>();

        Assert.NotNull(content);
        Assert.NotEmpty(content);

        // delete all
        await Parallel.ForEachAsync(content, async (item, token) =>
        {
            var deleteResponse = await client.DeleteAsync($"/todoItems/{item.Id}", token);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        });

        await TestGetAllEmpty();
    }

    private async Task GetAllAndDelete()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync("/todoItems");
        var content = await getResponse.Content.ReadFromJsonAsync<List<TodoItem>>() ?? [];
        if (content.Count == 0)
            return;

        await Parallel.ForEachAsync(content, async (item, token) => { await client.DeleteAsync($"/todoItems/{item.Id}", token); });
    }
}