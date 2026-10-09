using Microsoft.AspNetCore.Mvc;

namespace PizzaApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
}
