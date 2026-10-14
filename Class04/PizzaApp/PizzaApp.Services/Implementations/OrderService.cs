using MapsterMapper;
using PizzaApp.DataAccess.Repositories.Abstractions;
using PizzaApp.Domain.Entities;
using PizzaApp.Dtos.Orders;
using PizzaApp.Services.Abstractions;
using PizzaApp.Services.Extensions;
using PizzaApp.Shared.Exceptions;

namespace PizzaApp.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public OrderService(
        IOrderRepository orderRepository,
        IMapper mapper,
        ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        string? userId = _currentUser.IsAdmin ? null : _currentUser.Id;

        List<Order> orders = await _orderRepository.GetWithDetailsAsync(userId, cancellationToken);

        List<OrderDto> result = _mapper.Map<List<OrderDto>>(orders);

        return result;
    }

    public async Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), id);

        _currentUser.EnsureOwnerOrAdmin(order.UserId);

        OrderDto orderDto = _mapper.Map<OrderDto>(order);

        return orderDto;
    }

    public async Task<OrderDto> CreateAsync(AddOrderDto request, CancellationToken cancellationToken = default)
    {
        var order = _mapper.Map<Order>(request);
        order.UserId = _currentUser.Id;

        foreach (var pizza in order.Pizzas)
        {
            pizza.UserId = _currentUser.Id;
        }

        order.TotalPrice = order.Pizzas.Sum(pizza => pizza.Price);
        await _orderRepository.AddAsync(order, cancellationToken);

        return await GetByIdAsync(order.Id, cancellationToken);
    }

    public Task<OrderDto> UpdateAsync(int id, UpdateOrderDto request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<OrderDto> ChangeStatusAsync(int id, ChangeOrderStatusDto request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

}
