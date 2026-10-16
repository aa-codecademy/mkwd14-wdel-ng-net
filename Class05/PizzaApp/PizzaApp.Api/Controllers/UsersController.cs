using Microsoft.AspNetCore.Mvc;
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

}
