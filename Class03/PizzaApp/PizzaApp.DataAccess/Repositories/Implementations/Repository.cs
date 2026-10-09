using Microsoft.EntityFrameworkCore;
using PizzaApp.DataAccess.Context;
using PizzaApp.DataAccess.Repositories.Abstractions;
using PizzaApp.Domain.Common;

namespace PizzaApp.DataAccess.Repositories.Implementations;

public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected PizzaAppDbContext DbContext { get; init; }
    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    protected Repository(PizzaAppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (DbContext.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Update(entity);
        }

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
