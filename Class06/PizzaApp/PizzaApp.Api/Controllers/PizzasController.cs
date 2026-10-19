using Microsoft.AspNetCore.Mvc;
using PizzaApp.Dtos.Pizzas;
using PizzaApp.Services.Abstractions;

namespace PizzaApp.Api.Controllers;

public class PizzasController : ApiControllerBase
{
    private readonly IPizzaService _pizzaService;

    public PizzasController(IPizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PizzaDto>>> GetAll(CancellationToken cancellationToken)
    {
        var pizzas = await _pizzaService.GetAllAsync(cancellationToken);
        return Ok(pizzas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PizzaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var pizza = await _pizzaService.GetByIdAsync(id, cancellationToken);
        return Ok(pizza);
    }

    /// <summary>Saves a new pizza. Returns 201 Created with its URL in the Location header.</summary>
    [HttpPost]
    public async Task<ActionResult<PizzaDto>> Create(AddPizzaDto request, CancellationToken cancellationToken)
    {
        var pizza = await _pizzaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = pizza.Id }, pizza);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PizzaDto>> Update(int id, UpdatePizzaDto request, CancellationToken cancellationToken)
    {
        var pizza = await _pizzaService.UpdateAsync(id, request, cancellationToken);
        return Ok(pizza);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _pizzaService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
