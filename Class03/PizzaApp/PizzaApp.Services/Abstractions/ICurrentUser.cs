namespace PizzaApp.Services.Abstractions;

/// <summary>
/// Who is calling the API right now.
/// Defined here, but implemented in the Api project,
/// because only the Api knows about HTTP requests and JWT claims (Dependency Inversion: the D in SOLID).
/// </summary>
public interface ICurrentUser
{
    /// <summary>The logged-in user's id (the "sub" claim of the JWT).</summary>
    string Id { get; }
    bool IsAdmin { get; }
}
