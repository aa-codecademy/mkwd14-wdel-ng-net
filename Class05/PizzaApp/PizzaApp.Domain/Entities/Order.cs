using PizzaApp.Domain.Common;
using PizzaApp.Domain.Enums;

namespace PizzaApp.Domain.Entities;

public class Order : BaseEntity
{
    public string AddressTo { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public List<Pizza> Pizzas { get; set; } = [];
}
