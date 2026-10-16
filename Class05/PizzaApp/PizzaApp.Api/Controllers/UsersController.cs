using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaApp.Domain.Constants;
using PizzaApp.Dtos.Users;
using PizzaApp.Services.Abstractions;

namespace PizzaApp.Api.Controllers;

public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var user = await _userService.GetCurrentUserAsync();
        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateMe(UpdateUserDto request)
    {
        var user = await _userService.UpdateCurrentUserAsync(request);
        return Ok(user);
    }

    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangeMyPassword(ChangePasswordDto request)
    {
        await _userService.ChangeCurrentUserPasswordAsync(request);
        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        await _userService.DeleteCurrentUserAsync();
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        bool isAdmin = User.IsInRole(Roles.Admin);

        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }
}
