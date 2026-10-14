using Microsoft.AspNetCore.Mvc;
using PizzaApp.Dtos.Orders;
using PizzaApp.Services.Abstractions;

namespace PizzaApp.Api.Controllers;

public class OrdersController : ApiControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(AddOrderDto request, CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateAsync(request, cancellationToken);

        //return StatusCode(StatusCodes.Status201Created, order);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
}
