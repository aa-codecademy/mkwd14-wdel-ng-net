using Microsoft.EntityFrameworkCore;
using PizzaApp.DataAccess.Context;
using PizzaApp.DataAccess.Repositories.Abstractions;
using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Repositories.Implementations;

public class PizzaRepository : Repository<Pizza>, IPizzaRepository
{
    public PizzaRepository(PizzaAppDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Pizza>> GetSavedPizzasAsync(
        string? userId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Pizza> query = DbSet
           .AsNoTracking()
           .Where(pizza => pizza.OrderId == null);

        if (userId is not null)
        {
            query = query.Where(pizza => pizza.UserId == userId);
        }

        query = query.OrderByDescending(pizza => pizza.CreatedDate);

        return query.ToListAsync(cancellationToken);
    }

}
