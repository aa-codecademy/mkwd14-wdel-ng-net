using PizzaApp.Domain.Common;
using PizzaApp.Domain.Enums;

namespace PizzaApp.Domain.Entities;

public class Pizza : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<Ingredient> Ingredients { get; set; } = [];
    public string UserId { get; set; }
    public User User { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}
