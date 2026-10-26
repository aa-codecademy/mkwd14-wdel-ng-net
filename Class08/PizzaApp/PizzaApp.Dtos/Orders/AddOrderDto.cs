using System.ComponentModel.DataAnnotations;
using PizzaApp.Dtos.Pizzas;

namespace PizzaApp.Dtos.Orders;

/// <summary>
/// A new order. There is no price field: the server calculates the total from the pizzas.
/// </summary>
public class AddOrderDto
{
    [Required]
    [StringLength(200)]
    public string AddressTo { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [MinLength(1, ErrorMessage = "An order needs at least one pizza.")]
    public List<AddPizzaDto> Pizzas { get; set; } = [];
}
