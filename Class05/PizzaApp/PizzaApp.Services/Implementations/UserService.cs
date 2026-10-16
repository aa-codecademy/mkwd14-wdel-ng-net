using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using PizzaApp.Domain.Entities;
using PizzaApp.Dtos.Users;
using PizzaApp.Services.Abstractions;
using PizzaApp.Shared.Exceptions;

namespace PizzaApp.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public UserService(
        ICurrentUser currentUser,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _currentUser = currentUser;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        var user = await FindUserAsync(_currentUser.Id);
        return await ToUserDtoAsync(user);
    }

    public async Task ChangeCurrentUserPasswordAsync(ChangePasswordDto request)
    {
        var user = await FindUserAsync(_currentUser.Id);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        ThrowIfFailed(result, "Changing the password failed.");
    }

    public async Task<UserDto> UpdateCurrentUserAsync(UpdateUserDto request)
    {
        var user = await FindUserAsync(_currentUser.Id);

        user.UserName = request.UserName;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);

        ThrowIfFailed(result, "Updating the user failed.");
        var userDto = await ToUserDtoAsync(user);

        return userDto;
    }

    public async Task DeleteCurrentUserAsync()
    {
        var user = await FindUserAsync(_currentUser.Id);
        var result = await _userManager.DeleteAsync(user);
        ThrowIfFailed(result, "Deleting the user failed.");
    }

    #region Private Helpers

    private async Task<User> FindUserAsync(string id)
    {
        return await _userManager.FindByIdAsync(id)
            ?? throw new NotFoundException(nameof(User), id);
    }

    private static void ThrowIfFailed(IdentityResult result, string message)
    {
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description);
            throw new BadRequestException(message, errors);
        }
    }

    private async Task<UserDto> ToUserDtoAsync(User user)
    {
        var userDto = _mapper.Map<UserDto>(user);
        var userRoles = await _userManager.GetRolesAsync(user); // ["Admin", "Customer"]
        userDto.Roles = [.. userRoles];
        return userDto;
    }

    #endregion

}
