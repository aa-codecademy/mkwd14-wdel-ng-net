using PizzaApp.Dtos.Orders;

namespace PizzaApp.Services.Abstractions;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(AddOrderDto request, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateAsync(int id, UpdateOrderDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDto> ChangeStatusAsync(int id, ChangeOrderStatusDto request, CancellationToken cancellationToken = default);
}
