using PizzaApp.Dtos.Auth;
using PizzaApp.Dtos.Users;

namespace PizzaApp.Services.Abstractions;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
