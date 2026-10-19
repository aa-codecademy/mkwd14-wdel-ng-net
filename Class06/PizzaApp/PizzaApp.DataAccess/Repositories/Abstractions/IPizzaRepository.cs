using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Repositories.Abstractions;

public interface IPizzaRepository : IRepository<Pizza>
{
    Task<List<Pizza>> GetSavedPizzasAsync(string? userId, CancellationToken cancellationToken = default);
}
