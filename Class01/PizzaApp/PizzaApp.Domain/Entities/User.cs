using Microsoft.AspNetCore.Identity;

namespace PizzaApp.Domain.Entities;

public class User : IdentityUser
{
    public List<Order> Orders { get; set; } = [];
    public List<Pizza> Pizzas { get; set; } = [];
}
