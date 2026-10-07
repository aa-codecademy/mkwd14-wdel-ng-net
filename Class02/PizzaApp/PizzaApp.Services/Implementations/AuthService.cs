using Microsoft.AspNetCore.Identity;
using PizzaApp.Domain.Constants;
using PizzaApp.Domain.Entities;
using PizzaApp.Dtos.Auth;
using PizzaApp.Dtos.Users;
using PizzaApp.Services.Abstractions;
using PizzaApp.Services.Models;
using PizzaApp.Shared.Exceptions;

namespace PizzaApp.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        ITokenService tokenService,
        UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
        };

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new BadRequestException("Registration failed.", result.Errors.Select(error => error.Description));
            //throw new Exception("Registration failed.");
        }

        await _userManager.AddToRoleAsync(user, Roles.Customer);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Roles = [Roles.Customer]
        };

        return userDto;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        User? user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        TokenResult tokenResult = _tokenService.CreateToken(user, roles);

        return new LoginResponseDto
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresEt,
            UserName = user.UserName ?? string.Empty,
            Roles = [.. roles]
        };
    }

}
