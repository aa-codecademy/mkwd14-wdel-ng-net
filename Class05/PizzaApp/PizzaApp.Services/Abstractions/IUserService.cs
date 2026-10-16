using PizzaApp.Dtos.Users;

namespace PizzaApp.Services.Abstractions;

public interface IUserService
{
    Task<UserDto> GetCurrentUserAsync();
    Task<UserDto> UpdateCurrentUserAsync(UpdateUserDto request);
    Task ChangeCurrentUserPasswordAsync(ChangePasswordDto request);
    Task DeleteCurrentUserAsync();
}
