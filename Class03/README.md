# 🍕 Class 03: Pizzas, the CRUD pattern

> ⚙️ Backend · class 3 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md) · 🗄️ [Database diagram](../Class01/pizza-app-db-diagram.png)

First we finish class 2: **every error**, from any layer, comes back in **one JSON shape**. Then we build a complete API for **saved pizzas**, where every user sees and changes **only their own**.

## 🗺️ What we do today

1. 🔁 Recap, and `AddApi()`: the TODO in `Program.cs`

**🧯 Errors** (the end of class 2)

2. 📦 `ErrorResponse`: one JSON shape for every error
3. 🚨 `GlobalExceptionHandler`: one place that turns exceptions into HTTP responses
4. 📄 Handout: invalid DTOs get an `ErrorResponse` too

**📚 Reading and saving**

5. 📚 The generic `IRepository<T>` / `Repository<T>`, and `PizzaRepository`

**📨 What goes over the wire**

6. 📨 The pizza DTOs, and ingredients as text
7. 🔄 Mapster: from an entity to a DTO in one line

**🍕 The pizza endpoints**

8. 👤 `ICurrentUser`: who is calling?
9. 🍕 `PizzaService`: the list, and one pizza for its owner or an admin
10. 🎮 `PizzasController`: `GET`, `GET {id}` and `CancellationToken`
11. 🧪 Test with Scalar

🏠 Create, update and delete are the [homework](#homework).

---

## 💡 Used concepts

### 🧩 AddApi(): every layer registers its own services
The TODO from class 2. Like `AddDataAccess()` and `AddServices()`, the web layer gets one extension method, and `Program.cs` reads like a table of contents:
```csharp
builder.Services
    .AddDataAccess(builder.Configuration)   // DbContext, Identity, repositories
    .AddMappers()                           // Mapster (later today)
    .AddServices()                          // business logic
    .AddApi();                              // controllers, JSON, errors, JWT, OpenAPI, the current user
```
`AddApi()` takes over `AddControllers()`, `LowercaseUrls`, `AddJwtAuthentication()` and `AddOpenApiDocumentation()`, and it grows a few lines today.

---

<a id="errors"></a>
### 📦 ErrorResponse: one shape for every error
Our services already throw [our own exceptions](../Class02/README.md#errors), but nobody turns them into HTTP responses yet: a wrong password gives a **500** and an HTML page. From now on, whatever goes wrong, the client gets the same JSON, so it always knows where to find the message:
```json
{
  "statusCode": 400,
  "message": "Registration failed.",
  "errors": [ "Passwords must have at least one digit ('0'-'9')." ],
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01"
}
```

### 🚨 GlobalExceptionHandler
The **one** place where exceptions become HTTP responses. It implements `IExceptionHandler`, and a `switch` expression picks the status code:
```csharp
public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
{
    var statusCode = exception switch
    {
        BadRequestException => StatusCodes.Status400BadRequest,
        UnauthorizedException => StatusCodes.Status401Unauthorized,
        ForbiddenException => StatusCodes.Status403Forbidden,
        NotFoundException => StatusCodes.Status404NotFound,
        ConflictException => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError,
    };

    // … a 500 is logged with its stack trace, and the client only gets "An unexpected error occurred."

    httpContext.Response.StatusCode = statusCode;
    await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    return true;   // handled: nothing else runs
}
```
🔐 **A 500 never shows the details** (stack traces, SQL, file paths) to the client: they help an attacker. They go to the log, and the `traceId` connects the two.

Two lines switch it on:
```csharp
services.AddExceptionHandler<GlobalExceptionHandler>();   // AddApi()
app.UseExceptionHandler(_ => { });                         // Program.cs, the FIRST middleware
```
🤔 **Why the empty lambda?** Without ProblemDetails (.NET's own error format, which we don't use), .NET 10 asks for a fallback handler. Ours answers every exception, so the lambda never runs.

✅ **Invalid DTOs too:** `[ApiController]`'s automatic 400 has .NET's own shape. 📄 **Handout:** `InvalidModelStateResponseFactory` in `AddApi()` makes it an `ErrorResponse` as well (`"message": "Validation failed."`, one line per problem in `errors`).

---

### 📚 A generic repository
Every entity needs the same five operations. We write them **once**, for any type that inherits `BaseEntity`:
```csharp
public interface IRepository<TEntity>
    where TEntity : BaseEntity   // Pizza, Order: so we know it has an int Id
{
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
```
`Repository<TEntity>` implements them with EF Core. A specific repository **inherits** the CRUD and only adds its own queries, through the `protected` `DbSet`:
```csharp
public class PizzaRepository : Repository<Pizza>, IPizzaRepository
{
    // … the constructor passes the DbContext to base(dbContext)

    public Task<List<Pizza>> GetSavedPizzasAsync(string? userId, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(pizza => pizza.OrderId == null);

        if (userId is not null)   // null = everybody's pizzas (the admin)
        {
            query = query.Where(pizza => pizza.UserId == userId);
        }

        return query.OrderByDescending(pizza => pizza.CreatedDate).ToListAsync(cancellationToken);
    }
}
```
🍕 **A saved pizza** has `OrderId == null`: a recipe the user keeps for later. Once it's ordered (class 4), it belongs to the order.

Repositories are **Scoped**: one per HTTP request, the same lifetime as the `DbContext` they use.
```csharp
services.AddScoped<IPizzaRepository, PizzaRepository>();   // AddDataAccess()
```

### 👀 Tracking and AsNoTracking
EF Core **tracks** the entities it loads: it keeps a copy, and `SaveChangesAsync` compares and writes only what changed.

| | Used in | Why |
|---|---|---|
| `AsNoTracking()` | the lists | only read, never saved: faster, less memory |
| tracked (`FindAsync`) | `GetByIdAsync` | we often change the entity and save it right after |

So for a pizza from `GetByIdAsync`, `UpdateAsync` only has to save: change its properties, call `SaveChangesAsync`, and EF Core writes an `UPDATE` with just the changed columns.

### ⏹️ CancellationToken
ASP.NET Core gives every action a `CancellationToken` that fires when the client **gives up**: closes the tab, navigates away, times out. Pass it all the way down, and EF Core stops the database query too:
```
PizzasController.GetAll(cancellationToken)
  → PizzaService.GetAllAsync(cancellationToken)
    → PizzaRepository.GetSavedPizzasAsync(…, cancellationToken)
      → ToListAsync(cancellationToken)   // the query is cancelled
```
`= default` in the interfaces makes it optional for a caller that doesn't have one.

---

### 📨 The pizza DTOs
One class per direction:

| DTO | For | Fields |
|---|---|---|
| `AddPizzaDto` | the `POST` body | name, description, price, ingredients |
| `UpdatePizzaDto` | the `PUT` body | the same (today; they can change independently) |
| `PizzaDto` | every response | + `id`, `createdDate`, `updatedDate` |

The validation attributes from class 2, and two new ones:
```csharp
[Range(0.01, 1000)]
public decimal Price { get; set; }

[MinLength(1, ErrorMessage = "Choose at least one ingredient.")]
public List<Ingredient> Ingredients { get; set; } = [];
```
🔒 **No `UserId` in a request.** The owner comes from the token ([the current user](#current-user)). A client that could send a `userId` could save pizzas in somebody else's name.

### 🔤 Enums as text: JsonStringEnumConverter
By default an enum travels as its **number**: `"ingredients": [0, 1, 8]`. Hard to read, and the client has to know the numbers. One line in `AddApi()`:
```csharp
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
    });
```
```json
"ingredients": ["TomatoSauce", "Mozzarella", "Basil"]
```
`allowIntegerValues: false`: a number (`[0, 1]`) or an unknown name (`"Pineapple"`) → **400**.

⚠️ Only the JSON changes: the database still stores the **numbers** (an `integer[]` column). Never reorder or renumber the enum; add new values at the end.

### 🔄 Mapster
Copies the properties with the same name from one object to another, so no more `new UserDto { Id = user.Id, … }` by hand. The mappings live in config classes in `PizzaApp.Mappers`:
```csharp
public class PizzaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Pizza, PizzaDto>();         // entity → DTO (responses)
        config.NewConfig<AddPizzaDto, Pizza>();      // DTO → entity (requests)
        config.NewConfig<UpdatePizzaDto, Pizza>();
    }
}
```
`AddMappers()` finds every config and checks it when the app starts:
```csharp
var config = TypeAdapterConfig.GlobalSettings;
config.RequireExplicitMapping = true;                 // a pair nobody configured is an error, not a guess
config.Scan(typeof(DependencyInjection).Assembly);    // every IRegister in this project
config.Compile();                                     // fail fast: a broken mapping stops the app at startup

services.AddSingleton(config);
services.AddScoped<IMapper, ServiceMapper>();
```
A service asks for `IMapper` (namespace `MapsterMapper`):

| Call | Result |
|---|---|
| `_mapper.Map<PizzaDto>(pizza)` | a **new** `PizzaDto` |
| `_mapper.Map<List<PizzaDto>>(pizzas)` | a **new list**: every item is mapped with the `Pizza → PizzaDto` config, no extra config needed |
| `_mapper.Map(request, pizza)` | copies the request **onto the existing** `pizza` (for an update) |

`AuthService.RegisterAsync` switches to Mapster too. A `User` has no `Roles` property, so `UserMappingConfig` ignores it, and the service fills it in:
```csharp
var userDto = _mapper.Map<UserDto>(user);
userDto.Roles = [Roles.Customer];
```

---

<a id="current-user"></a>
### 👤 ICurrentUser: who is calling?
`PizzaService` needs the logged-in user's id. But a service doesn't know about HTTP, tokens or `HttpContext`, and it shouldn't. So **Services defines** what it needs:
```csharp
// PizzaApp.Services/Abstractions
public interface ICurrentUser
{
    string Id { get; }      // the "sub" claim
    bool IsAdmin { get; }   // a "role" claim with Admin
}
```
and **Api implements** it, because only the Api knows the request:
```csharp
// PizzaApp.Api/Security
public class CurrentUser : ICurrentUser
{
    // … IHttpContextAccessor from the constructor

    public string Id =>
        Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedException("You are not logged in.");

    public bool IsAdmin => Principal?.IsInRole(Roles.Admin) ?? false;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;
}
```
```csharp
services.AddHttpContextAccessor();                  // AddApi()
services.AddScoped<ICurrentUser, CurrentUser>();
```
🔄 **Dependency Inversion**, the D in SOLID: Services never references Api. Api plugs its implementation into the interface that Services owns. The same `PizzaService` would work in a console app or a test, with another `ICurrentUser`.

### 🍕 PizzaService: the CRUD pattern
Every method follows the same steps: **find → check → change → map**. The service throws, and the `GlobalExceptionHandler` answers: no try/catch, no status codes.
```csharp
public async Task<PizzaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
{
    var pizza = await FindPizzaAsync(id, cancellationToken);   // 404 "Pizza with id 7 was not found."

    if (!_currentUser.IsAdmin && _currentUser.Id != pizza.UserId)
    {
        throw new ForbiddenException();                          // 403: somebody else's pizza
    }

    return _mapper.Map<PizzaDto>(pizza);
}

private async Task<Pizza> FindPizzaAsync(int id, CancellationToken cancellationToken)
{
    return await _pizzaRepository.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException(nameof(Pizza), id);
}
```
👑 **The list:** a customer gets their own saved pizzas, the admin gets everybody's: `_currentUser.IsAdmin ? null : _currentUser.Id` goes to `GetSavedPizzasAsync`.

### 🎮 PizzasController: REST status codes

| Request | Success |
|---|---|
| `GET /api/pizzas` | **200** + the list |
| `GET /api/pizzas/{id}` | **200** + the pizza |
| 🏠 `POST /api/pizzas` | **201 Created** + the pizza + a `Location` header |
| 🏠 `PUT /api/pizzas/{id}` | **200** + the updated pizza |
| 🏠 `DELETE /api/pizzas/{id}` | **204 No Content**: nothing left to return |

```csharp
[HttpGet("{id:int}")]
public async Task<ActionResult<PizzaDto>> GetById(int id, CancellationToken cancellationToken)
{
    var pizza = await _pizzaService.GetByIdAsync(id, cancellationToken);
    return Ok(pizza);
}
```
- 📍 **201 Created + `Location`:** the answer to a create also says where the new resource lives: `Location: http://localhost:5004/api/pizzas/7`.
- 🫙 **204 No Content:** it worked, and there's no body.
- 🔢 `{id:int}` is a route **constraint**: `/api/pizzas/abc` doesn't match the route, so it's a 404 and the action never runs.
- 🏷️ The actions have **no** `Async` suffix. ASP.NET Core cuts `Async` off action names, so code that points to an action by name, like `nameof(GetByIdAsync)`, finds nothing: *"No route matches the supplied values"*.
- 🔒 No `[AllowAnonymous]`: the fallback policy from class 2 makes every pizza endpoint need a token.

### 🧪 The .http file
📄 **Handout, to test the homework:** `PizzaApp.Api.http`, requests you send straight from the editor (Visual Studio: **Send request** above a request; VS Code: the **REST Client** extension). A named request keeps its response, so the next requests reuse the token and the new id:
```http
@baseUrl = http://localhost:5004/api

### Log in → 200 + a token
# @name customerLogin
POST {{baseUrl}}/auth/login
Content-Type: application/json

{ "userName": "ana", "password": "Ana123!" }

### Create a pizza → 201
# @name createPizza
POST {{baseUrl}}/pizzas
Authorization: Bearer {{customerLogin.response.body.$.token}}
Content-Type: application/json

{ "name": "Margherita", "price": 7.50, "ingredients": ["TomatoSauce", "Mozzarella", "Basil"] }

### Read it back → 200
GET {{baseUrl}}/pizzas/{{createPizza.response.body.$.id}}
Authorization: Bearer {{customerLogin.response.body.$.token}}
```
🚫 **The 403:** register a second user, log in as them, and ask for Ana's pizza.

---

<a id="homework"></a>
## 🏠 Homework
The two TODOs in `PizzaService.cs`. We go through them at the start of class 4.

1️⃣ **The ownership check, written once.** Make the "owner or admin" check in `GetByIdAsync` reusable, so every service that needs it can call it (tip: an extension method).

2️⃣ **Create, update and delete a pizza.**

| Endpoint | Success |
|---|---|
| `POST /api/pizzas` | **201** + the new pizza + a `Location` header |
| `PUT /api/pizzas/{id}` | **200** + the updated pizza |
| `DELETE /api/pizzas/{id}` | **204** |

- 👤 a new pizza belongs to the logged-in user
- 🔒 only its owner or an admin may change or delete a pizza: anyone else → **403**, an unknown id → **404**
- 🧾 a pizza that is part of an order can't be changed or deleted → **409**

## ✅ By the end of the class
- 🧯 every error is an `ErrorResponse`: a wrong password → 401, a weak password → 400 with Identity's messages, an empty form → 400 "Validation failed."
- 🍕 `GET /api/pizzas` (a customer gets their own saved pizzas, the admin everybody's) and `GET /api/pizzas/{id}`
- 🚫 an unknown id → 404, no token → 401
- 🧱 the solution builds with 0 warnings

---

🔜 **Next class:** Orders, roles and business rules. We start with the homework, then `Include`, a total that the server computes, and the order status workflow.
