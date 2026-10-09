using System.ComponentModel.DataAnnotations;

namespace PizzaApp.Shared.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public const string RoleClaimType = "role";

    [Required]
    public string Issuer { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = string.Empty;
    [Range(1, 1000)]
    public int ExpirationInMinutes { get; set; } = 60;
}
