using PizzaApp.Domain.Entities;
using PizzaApp.Services.Models;

namespace PizzaApp.Services.Abstractions;

public interface ITokenService
{
    TokenResult CreateToken(User user, IEnumerable<string> roles);
}
