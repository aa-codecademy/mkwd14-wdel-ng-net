namespace PizzaApp.Shared.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You don't have permission to do this.") : base(message)
    {
    }
}
