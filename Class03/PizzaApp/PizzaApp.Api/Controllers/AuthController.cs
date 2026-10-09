using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaApp.Dtos.Auth;
using PizzaApp.Dtos.Users;
using PizzaApp.Services.Abstractions;

namespace PizzaApp.Api.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _authService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}
