using PizzaApp.Services.Abstractions;
using PizzaApp.Shared.Exceptions;

namespace PizzaApp.Services.Extensions;

public static class CurrentUserExtensions
{
    /// <summary>
    /// The ownership rule, written once and used everywhere:
    /// only the owner of a pizza/order, or an Admin, may see or change it.
    /// </summary>
    public static void EnsureOwnerOrAdmin(this ICurrentUser currentUser, string ownerId)
    {
        if (!currentUser.IsAdmin && currentUser.Id != ownerId)
        {
            throw new ForbiddenException();
        }
    }
}
