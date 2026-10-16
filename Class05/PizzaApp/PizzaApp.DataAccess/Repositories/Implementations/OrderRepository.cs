using Microsoft.EntityFrameworkCore;
using PizzaApp.DataAccess.Context;
using PizzaApp.DataAccess.Repositories.Abstractions;
using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Repositories.Implementations;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(PizzaAppDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Order>> GetWithDetailsAsync(string? userId, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = DbSet
            .AsNoTracking()
            .Include(order => order.Pizzas)
            .Include(order => order.User);

        if (userId is not null)
        {
            query = query.Where(order => order.UserId == userId);
        }

        query = query.OrderByDescending(order => order.CreatedDate);

        return query.ToListAsync(cancellationToken);
    }

    public Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(order => order.Pizzas)
            .Include(order => order.User)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

}
