using PizzaApp.Dtos.Pizzas;

namespace PizzaApp.Services.Abstractions;

public interface IPizzaService
{
    Task<List<PizzaDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PizzaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PizzaDto> CreateAsync(AddPizzaDto request, CancellationToken cancellationToken = default);
    Task<PizzaDto> UpdateAsync(int id, UpdatePizzaDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
