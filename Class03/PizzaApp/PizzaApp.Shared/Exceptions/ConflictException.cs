namespace PizzaApp.Shared.Exceptions;

/// <summary>
/// 409 Conflict: the request is valid, but not in the item's current state
/// (e.g. a delivered order can't be cancelled).
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message)
    {
    }
}
