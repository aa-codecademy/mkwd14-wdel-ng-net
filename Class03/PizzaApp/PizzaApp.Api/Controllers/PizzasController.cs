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


}
