using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace FlaschenpostApi.Repository;

public abstract class BaseRepository<TType>(DbContext dbContext) where TType : class
{
    public virtual async Task AddAsync(TType entity)
    {
        await dbContext.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public virtual async Task AddRangeAsync(IEnumerable<TType> entities)
    {
        await dbContext.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public virtual async Task<TType?> GetAsync(int id)
    {
        return await dbContext.Set<TType>().FindAsync(id);
    }

    public virtual async Task<List<TType>> GetAsync(Expression<Func<TType, bool>> predicate)
    {
        return await dbContext.Set<TType>().Where(predicate).ToListAsync();
    }

    public virtual async Task<List<TType>> GetAllAsync()
    {
        return await dbContext.Set<TType>().ToListAsync();
    }

    public abstract Task<bool> UpdateAsync(int id, TType other);

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity is null)
            return false;

        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
        return true;
    }
}