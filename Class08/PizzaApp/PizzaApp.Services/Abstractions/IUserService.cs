using PizzaApp.Dtos.Users;

namespace PizzaApp.Services.Abstractions;

public interface IUserService
{
    Task<UserDto> GetCurrentUserAsync();
    Task<UserDto> UpdateCurrentUserAsync(UpdateUserDto request);
    Task ChangeCurrentUserPasswordAsync(ChangePasswordDto request);
    Task DeleteCurrentUserAsync();

    // Admin only
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(string id);
    Task<UserDto> SetRoleAsync(string id, string role);
    Task DeleteAsync(string id);
}
