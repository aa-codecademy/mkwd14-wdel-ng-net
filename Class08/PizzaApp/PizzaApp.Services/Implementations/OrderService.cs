using MapsterMapper;
using PizzaApp.DataAccess.Repositories.Abstractions;
using PizzaApp.Domain.Entities;
using PizzaApp.Domain.Enums;
using PizzaApp.Dtos.Orders;
using PizzaApp.Services.Abstractions;
using PizzaApp.Services.Extensions;
using PizzaApp.Services.Rules.Order;
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
        var order = await FindOrderAsync(id, cancellationToken);

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

    public async Task<OrderDto> UpdateAsync(int id, UpdateOrderDto request, CancellationToken cancellationToken = default)
    {
        var order = await FindOrderAsync(id, cancellationToken);
        _currentUser.EnsureOwnerOrAdmin(order.UserId);

        if (order.Status != OrderStatus.Pending)
        {
            throw new ConflictException($"Only pending orders can be changed. This order is {order.Status}.");
        }

        // Copies the new address and description onto the tracked order
        _mapper.Map(request, order);
        order.UpdatedDate = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> ChangeStatusAsync(
        int id, ChangeOrderStatusDto request,
        CancellationToken cancellationToken = default)
    {
        OrderStatus newStatus = request.Status ?? throw new BadRequestException("The new status is required.");

        var order = await FindOrderAsync(id, cancellationToken);
        _currentUser.EnsureOwnerOrAdmin(order.UserId);

        // Current Status => order.Status
        // New Status => request.Status

        if (!OrderStatusRules.CanChange(order.Status, newStatus, _currentUser.IsAdmin))
        {
            throw new ConflictException($"An order can't go from {order.Status} to {newStatus}.");
        }

        order.Status = newStatus;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // Only Admins get here: [Authorize(Roles = Roles.Admin)] on the controller action
        var order = await FindOrderAsync(id, cancellationToken);

        // The database deletes its pizzas too (cascade delete)
        await _orderRepository.DeleteAsync(order, cancellationToken);
    }

    private async Task<Order> FindOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), id);
    }
}
