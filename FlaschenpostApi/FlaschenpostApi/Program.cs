using FlaschenpostApi.Entities;
using FlaschenpostApi.Persistence;
using FlaschenpostApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace FlaschenpostApi;

public class Program
{
    public static void Main(string[] args)
    {
        var program = new Program();
        program.Run(args);
    }

    private void Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        RegisterServices(builder);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            const string endpointName = "v1";
            app.UseSwagger(options => { options.RouteTemplate = $"swagger/{{documentName}}/flaschenpostOpenApi_{endpointName}.json"; });
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = string.Empty;
                options.SwaggerEndpoint($"/swagger/{endpointName}/flaschenpostOpenApi_{endpointName}.json", endpointName);
            });
            app.MapSwagger();
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        RegisterApiEndpoints(app);
        app.Run();
    }

    private void RegisterServices(WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDbContext<TodoContext>(options => { options.UseInMemoryDatabase("TaskList"); });
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddTransient<TodoItemRepository>();
    }

    private void RegisterApiEndpoints(WebApplication app)
    {
        const string todoItems = "todoItems";
        app.MapGet($"/{todoItems}", async (TodoItemRepository repo) => await repo.GetAllAsync())
            .Produces<List<TodoItem>>();

        app.MapGet($"/{todoItems}/open", async (TodoItemRepository repo) => await repo.GetAsync(todoItem => todoItem.CompletedDateTime == null))
            .Produces<List<TodoItem>>();

        app.MapGet($"/{todoItems}/completed", async (TodoItemRepository repo) => await repo.GetAsync(todoItem => todoItem.CompletedDateTime != null))
            .Produces<List<TodoItem>>();

        app.MapGet($"/{todoItems}/{{id:int}}", async (int id, TodoItemRepository repo) =>
                await repo.GetAsync(id)
                    is { } todoItem
                    ? Results.Ok(todoItem)
                    : Results.NotFound())
            .Produces<TodoItem>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost($"/{todoItems}", async (List<TodoItem> items, TodoItemRepository repo) =>
            {
                try
                {
                    await repo.AddRangeAsync(items);
                    return Results.NoContent();
                }
                catch (Exception)
                {
                    return Results.BadRequest("the transmitted data does not meet the requirements");
                }
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut($"/{todoItems}/{{id:int}}", async (int id, TodoItem todoItem, TodoItemRepository repo) =>
            {
                var canUpdate = await repo.UpdateAsync(id, todoItem);
                return canUpdate ? Results.NoContent() : Results.NotFound();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete($"/{todoItems}/{{id:int}}", async (int id, TodoItemRepository repo) =>
            {
                var canDelete = await repo.DeleteAsync(id);
                return canDelete ? Results.NoContent() : Results.NotFound();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}