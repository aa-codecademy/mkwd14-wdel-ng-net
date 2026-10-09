using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using PizzaApp.Domain.Constants;
using PizzaApp.Services.Abstractions;
using PizzaApp.Shared.Exceptions;

namespace PizzaApp.Api.Security;

/// <summary>
/// Reads the logged-in user from the JWT of the current HTTP request.
/// Services only see the ICurrentUser interface, never HttpContext.
/// </summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    // IHttpContextAccessor gives access to the current HTTP request, which contains the JWT in its headers.
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public string Id => Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedException("You are not logged in.");

    public bool IsAdmin => Principal?.IsInRole(Roles.Admin) ?? false;
}
