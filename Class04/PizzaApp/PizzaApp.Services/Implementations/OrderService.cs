using MapsterMapper;
using PizzaApp.DataAccess.Repositories.Abstractions;
using PizzaApp.Dtos.Orders;
using PizzaApp.Services.Abstractions;

namespace PizzaApp.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<OrderDto> CreateAsync(AddOrderDto request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<OrderDto> UpdateAsync(int id, UpdateOrderDto request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<OrderDto> ChangeStatusAsync(int id, ChangeOrderStatusDto request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

}
