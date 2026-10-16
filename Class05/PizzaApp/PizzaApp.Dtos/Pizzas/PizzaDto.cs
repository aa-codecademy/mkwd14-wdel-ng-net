using PizzaApp.Domain.Enums;

namespace PizzaApp.Dtos.Pizzas;

public class PizzaDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<Ingredient> Ingredients { get; set; } = [];
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
