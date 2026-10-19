using System.ComponentModel.DataAnnotations;
using PizzaApp.Domain.Constants;

namespace PizzaApp.Dtos.Users;

public class SetUserRoleDto
{
    [Required]
    [AllowedValues(Roles.Admin, Roles.Customer, ErrorMessage = "Role must be Admin or Customer.")]
    public string Role { get; set; } = string.Empty;
}
