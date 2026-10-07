using Microsoft.AspNetCore.Identity;

namespace PizzaApp.Domain.Entities;

/// <summary>
/// A user of the app. IdentityUser already brings Id, UserName, Email, PasswordHash, ...
/// </summary>
public class User : IdentityUser
{
    public List<Order> Orders { get; set; } = [];
    public List<Pizza> Pizzas { get; set; } = [];
}
