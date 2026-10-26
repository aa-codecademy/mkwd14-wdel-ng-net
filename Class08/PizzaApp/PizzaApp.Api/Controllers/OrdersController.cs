using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaApp.Domain.Constants;
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderDto>> Update(int id, UpdateOrderDto request, CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateAsync(id, request, cancellationToken);
        return Ok(order);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> ChangeStatus(
        int id, ChangeOrderStatusDto request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(order);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _orderService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
