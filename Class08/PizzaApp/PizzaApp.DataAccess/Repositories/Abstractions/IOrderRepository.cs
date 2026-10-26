using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Repositories.Abstractions;

public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetWithDetailsAsync(string? userId, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
