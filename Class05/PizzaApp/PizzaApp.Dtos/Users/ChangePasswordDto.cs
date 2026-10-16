using System.ComponentModel.DataAnnotations;

namespace PizzaApp.Dtos.Users;

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}
