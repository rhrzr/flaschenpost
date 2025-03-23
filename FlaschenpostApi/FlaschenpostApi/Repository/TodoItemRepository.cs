using FlaschenpostApi.Entities;
using FlaschenpostApi.Persistence;

namespace FlaschenpostApi.Repository;

public class TodoItemRepository(TodoContext todoContext) : BaseRepository<TodoItem>(todoContext)
{
    public override async Task<bool> UpdateAsync(int id, TodoItem other)
    {
        var entity = await GetAsync(id);
        if (entity is null)
            return false;

        entity.Update(other);
        await todoContext.SaveChangesAsync();
        return true;
    }
}