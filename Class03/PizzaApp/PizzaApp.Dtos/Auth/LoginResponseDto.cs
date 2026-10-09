namespace PizzaApp.Dtos.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}
