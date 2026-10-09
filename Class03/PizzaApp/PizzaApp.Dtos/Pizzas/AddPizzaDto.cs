using System.ComponentModel.DataAnnotations;
using PizzaApp.Domain.Enums;

namespace PizzaApp.Dtos.Pizzas;

public class AddPizzaDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 1000)]
    public decimal Price { get; set; }

    [MinLength(1, ErrorMessage = "Choose at least one ingredient")]
    public List<Ingredient> Ingredients { get; set; } = [];
}
