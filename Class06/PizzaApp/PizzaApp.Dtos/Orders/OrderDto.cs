using PizzaApp.Domain.Enums;
using PizzaApp.Dtos.Pizzas;

namespace PizzaApp.Dtos.Orders;

public class OrderDto
{
    public int Id { get; set; }

    public string AddressTo { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal TotalPrice { get; set; }

    public OrderStatus Status { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string? UserName { get; set; }

    public List<PizzaDto> Pizzas { get; set; } = [];

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
