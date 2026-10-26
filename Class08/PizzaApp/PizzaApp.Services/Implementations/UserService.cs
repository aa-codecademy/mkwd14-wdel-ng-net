using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    #region Logged in user

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

    #endregion

    #region Admin only

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            // Should be optimized in the future if the number of users increases
            userDtos.Add(await ToUserDtoAsync(user));
        }

        return userDtos;
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        var user = await FindUserAsync(id);
        return await ToUserDtoAsync(user);
    }

    public async Task<UserDto> SetRoleAsync(string id, string role)
    {
        var user = await FindUserAsync(id);

        // A user has exactly one role in this app: replace the old one
        var currentRoles = await _userManager.GetRolesAsync(user);

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        ThrowIfFailed(removeResult, "Removing the old role failed.");

        var addResult = await _userManager.AddToRoleAsync(user, role);
        ThrowIfFailed(addResult, "Setting the role failed.");

        return await ToUserDtoAsync(user);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await FindUserAsync(id);

        // The database deletes the user's orders and pizzas too (cascade delete)
        var result = await _userManager.DeleteAsync(user);
        ThrowIfFailed(result, "Deleting the user failed.");
    }

    #endregion

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
