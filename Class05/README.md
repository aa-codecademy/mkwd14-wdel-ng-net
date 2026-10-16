# 👥 Class 05: Users, admin and wrap-up

> ⚙️ Backend · class 5 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md) · 🗄️ [Database diagram](../Class01/pizza-app-db-diagram.png)

The last backend class. Every user manages **their own profile**, and the admin manages **everybody**. Then we look back at the code of five classes, and open the API to the **Angular app**.

## 🗺️ What we do today

1. 🔁 Recap: the homework (change and delete an order)

**🧪 Orders, tested (the end of class 4)**

2. 📄 The orders in the `.http` file: the customer, the admin, and the refused cases last

**👥 Users**

3. 🙋 `/api/users/me`: see and change your own profile, through `UserManager`
4. 👑 Admin user management: list the users, set a role (`[AllowedValues]`), delete a user

**🔍 Looking back**

5. 🧪 The full `.http` scenario, from register to admin
6. 🧱 Code review: SOLID, DRY and KISS in our code, and `Program.cs` as the composition root

**🅰️ Ready for Angular**

7. 🌍 CORS: let the Angular app call the API from the browser
8. 📖 The API, as the Angular app sees it
9. 🖥️ What a single-page app is, and a first look at the finished Angular app

🏠 One user, changing your password and deleting your account are the [homework](#homework).

---

## 💡 Used concepts

### 👤 UserManager: Identity's own repository
We don't write a `UserRepository`. Identity's `UserManager<User>` already is one, and it also validates, hashes and normalizes:

| We need | `UserManager` gives |
|---|---|
| one user | `FindByIdAsync(id)`: `null` when the id is unknown |
| all users | `Users`: an `IQueryable<User>`, so `OrderBy` and `ToListAsync` work as on a `DbSet` |
| save a change | `UpdateAsync(user)`: checks that the username and email are still unique, updates the normalized columns, saves |
| a user's roles | `GetRolesAsync(user)` |
| change a role | `RemoveFromRolesAsync(user, roles)`, then `AddToRoleAsync(user, role)` |
| delete | `DeleteAsync(user)` |

🚦 **A result, not an exception:** `UpdateAsync` and `DeleteAsync` (like `CreateAsync` in class 2) return an `IdentityResult`. One private helper turns a failure into our `BadRequestException`, with Identity's messages in `errors`:
```csharp
private static void ThrowIfFailed(IdentityResult result, string message)
{
    if (!result.Succeeded)
    {
        throw new BadRequestException(message, result.Errors.Select(error => error.Description));
    }
}
```
A taken username → **400** *"Username 'bob' is already taken."*

🎭 **Roles aren't a property of `User`:** they live in `AspNetUserRoles`. `UserMappingConfig` ignores `Roles` (class 3), so the service asks for them:
```csharp
private async Task<UserDto> ToUserDtoAsync(User user)
{
    var userDto = _mapper.Map<UserDto>(user);
    userDto.Roles = [.. await _userManager.GetRolesAsync(user)];

    return userDto;
}
```
🔒 The response is always a `UserDto` (`id`, `userName`, `email`, `roles`), never the `User` entity, so the password hash can't leak.

### 🙋 /me: the id comes from the token
```
GET /api/users/me      → the logged-in user
GET /api/users/{id}    → any user (the admin)
```
- 🎟️ **No id in the URL:** the service takes it from the token (`_currentUser.Id`, the `sub` claim). Nobody can ask for somebody else's profile this way, so there's nothing to check.
- 🧭 **`me` next to `{id}`:** when both routes exist for the same HTTP method, the **fixed segment wins** over the parameter, so `/users/me` is never read as "the user whose id is `me`".
- 📛 **A new username, the same token:** the token still carries the old `name` until the next login. Everything keeps working, because the API recognizes you by `sub`: your id, which never changes.
- 🛡️ **The DTO decides what can change:** `UpdateUserDto` has only `userName` and `email`. A body that also sends `"roles": ["Admin"]` changes nothing: the field isn't in the DTO, so it's ignored.

### 👑 Admin endpoints on a mixed controller
`UsersController` serves everybody (`/me`) **and** the admin, so `[Authorize(Roles = Roles.Admin)]` goes on each admin **action**, not on the class:
```csharp
[HttpGet]
[Authorize(Roles = Roles.Admin)]
public async Task<ActionResult<List<UserDto>>> GetAll()
```
🎟️ **A JWT is a snapshot** (class 4), in both directions:
- the admin makes Bob an Admin: Bob's **current** token still says Customer (403) until he logs in again;
- the admin deletes a user: that user's token is still valid until it expires. It passes the JWT check, and then `/users/me` answers 404, because the user is gone.

### ✅ [AllowedValues]: only these values
```csharp
public class SetUserRoleDto
{
    [Required]
    [AllowedValues(Roles.Admin, Roles.Customer, ErrorMessage = "Role must be Admin or Customer.")]
    public string Role { get; set; } = string.Empty;
}
```
- A validation attribute (since .NET 8): any other value → **400**, before the controller runs, like `[Required]`.
- 🔠 It compares exactly: `"Chef"` → 400, and so does `"admin"`.
- ❓ **Why not an enum?** Roles are strings everywhere else: in `AspNetRoles`, in the token, and in our `Roles` constants.

### 🗑️ Deleting a user deletes their data
`_userManager.DeleteAsync(user)` deletes one row from `AspNetUsers`. PostgreSQL does the rest: the cascade deletes from class 1 remove the user's orders, their pizzas and their roles.

### 📋 The user endpoints

| Request | Who | Success |
|---|---|---|
| `GET /api/users/me` | logged in | **200** + your `UserDto` |
| `PUT /api/users/me` | logged in | **200** + the updated `UserDto` |
| `GET /api/users` | Admin | **200** + every user, by username |
| `PUT /api/users/{id}/role` | Admin | **200** + the user with the new role |
| `DELETE /api/users/{id}` | Admin | **204 No Content** |
| 🏠 `GET /api/users/{id}` | Admin | **200** + the user |
| 🏠 `PUT /api/users/me/password` | logged in | **204 No Content** |
| 🏠 `DELETE /api/users/me` | logged in | **204 No Content** |

🔑 Identity's ids are strings (`"3ee410fc-52e0-…"`), so the user routes have `{id}`, not `{id:int}`.

---

### 🧪 The whole story in one file
📄 The `.http` file now covers the finished API: register → pizzas → orders → the admin → users. Run it from top to bottom, as the customer, as Bob and as the admin. Its last section tests your homework, and its last request deletes ana, so the file runs again from the top.

### 🧱 Looking back: SOLID, DRY and KISS in our code

| | Means | Where it shows up |
|---|---|---|
| **S** Single responsibility | one reason to change | controllers only translate HTTP, services hold the rules, repositories talk to the database; `OrderStatusRules` knows only the status arrows; `GlobalExceptionHandler` is the only place where an exception becomes a response |
| **O** Open/closed | add code, don't edit it | a new entity configuration or mapping config is a new class, found by `ApplyConfigurationsFromAssembly` and Mapster's `Scan`; `OrderRepository` extends `Repository<T>` without changing it |
| **L** Liskov substitution | a child works wherever its parent does | every `AppException` child goes through the same handler; `OrderRepository` works as an `IRepository<Order>` |
| **I** Interface segregation | small interfaces | `ICurrentUser` has two members; one service interface per area instead of one big service |
| **D** Dependency inversion | depend on abstractions | services get `IOrderRepository`, `IMapper` and `ICurrentUser` through the constructor; `ICurrentUser` is defined in Services and implemented in Api |

- ♻️ **DRY:** one ownership rule (`EnsureOwnerOrAdmin`), one error shape (`ErrorResponse`), one base controller and one generic repository, the `FindXxxAsync` and `ThrowIfFailed` helpers, every package version once in `Directory.Packages.props`.
- 💋 **KISS:** one role per user; no `UserRepository` (`UserManager` is enough); one small roles query per user in the admin list (fine for a pizza shop); no `try`/`catch` in controllers; and everything in the "left out" list: refresh tokens, pagination, a server-side price list.

🧩 **The TODO from class 4** (*"Think of ways to improve this method"*): one line per arrow of the status diagram, with a `switch` on a **tuple**, where `or` joins two values in one pattern:
```csharp
return (currentStatus, newStatus) switch
{
    (OrderStatus.Pending, OrderStatus.Preparing) => true,
    (OrderStatus.Preparing, OrderStatus.Delivered) => true,
    (OrderStatus.Pending or OrderStatus.Preparing, OrderStatus.Cancelled) => true,
    _ => false,   // everything else is refused
};
```

🏗️ **`Program.cs` is the composition root:** the only place that knows every layer. Each layer registers its own services in its own `DependencyInjection.cs`, and `Program.cs` only plugs them together:
```csharp
builder.Services
    .AddDataAccess(builder.Configuration)   // DbContext, Identity, repositories
    .AddMappers()                           // Mapster configs + IMapper
    .AddServices()                          // business logic
    .AddApi(builder.Configuration);         // controllers, JSON, errors, JWT, CORS, current user
```
Then the middleware, where the **order** matters: errors first, then CORS, then "who are you?", then "are you allowed?":
```
UseExceptionHandler → UseCors → UseAuthentication → UseAuthorization → MapControllers
```

---

### 🌍 CORS: who may call the API from a browser
The Angular app runs on `http://localhost:4200`, the API on `http://localhost:5004`. An **origin** is scheme + host + **port**, so these are two origins, and the browser won't give the API's answer to the page's JavaScript unless the API allows it.
- 🛂 **The browser enforces CORS, not the server.** Scalar, the `.http` file and Postman ignore it. It protects users from other websites that would call an API with *their* browser. It doesn't protect the API: the JWT does that.
- ✈️ **The preflight:** before a request with a JSON body or an `Authorization` header, the browser first sends an `OPTIONS` request (*"may `localhost:4200` send a POST with these headers?"*). The API answers with `Access-Control-Allow-…` headers, or the browser stops there.
- 🔍 **Without CORS:** DevTools shows *"blocked by CORS policy"*, and Angular sees an error with **`status: 0`**: the API did answer, but the browser hides the response.

```csharp
public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
{
    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

    services.AddCors(options => options.AddPolicy(FrontendPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

    return services;
}
```
```json
"Cors": {
  "AllowedOrigins": [ "http://localhost:4200" ]
}
```
- 📝 **The origins are settings:** when the app is deployed, its real address goes into `appsettings.json`, with no code change.
- 🚫 **No `AllowAnyOrigin()`:** then any website could call our API from a visitor's browser.
- 🍪 **No `AllowCredentials()`:** that's for cookies. Our token travels in the `Authorization` header, which `AllowAnyHeader()` already allows.
- 🧭 **`app.UseCors(…)` before `UseAuthentication`:** the preflight has no token, so CORS must answer it before the fallback policy says 401. The 401 and 403 answers need the CORS headers too, or Angular sees `status: 0` instead of 401.
- ▶️ **Run the API with the `http` profile:** Angular calls `http://localhost:5004`.

### 📖 The API, as the Angular app sees it
- 🌐 Base URL `http://localhost:5004/api`, JSON in camelCase.
- 🎟️ Every request except register and login sends `Authorization: Bearer <token>`. A token lasts 60 minutes.
- 🔤 Enums are text (`"TomatoSauce"`, `"Pending"`), dates are in UTC, and the server computes an order's total.
- 🧯 Every error has the same body: `{ statusCode, message, errors, traceId }`. Show `message`; `errors` lists the validation and Identity messages. A 401 or 403 from the middleware has **no body**, so check the status code first.

| Area | Endpoints |
|---|---|
| 🔑 Auth (anonymous) | `POST /auth/register` { userName, email, password } → 201 `UserDto` · `POST /auth/login` { userName, password } → 200 { token, expiresAt, userName, roles } |
| 🙋 Me | `GET /users/me` → `UserDto` · `PUT /users/me` { userName, email } · 🏠 `PUT /users/me/password` { currentPassword, newPassword } → 204 · 🏠 `DELETE /users/me` → 204 |
| 🍕 Saved pizzas | `GET /pizzas` · `GET /pizzas/{id}` · `POST /pizzas` { name, description?, price, ingredients } → 201 · `PUT /pizzas/{id}` · `DELETE /pizzas/{id}` → 204 |
| 📦 Orders | `GET /orders` · `GET /orders/{id}` · `POST /orders` { addressTo, description?, pizzas } → 201 · `PUT /orders/{id}` { addressTo, description? } · `PATCH /orders/{id}/status` { status } |
| 👑 Admin | `DELETE /orders/{id}` → 204 · `GET /users` · 🏠 `GET /users/{id}` · `PUT /users/{id}/role` { role } · `DELETE /users/{id}` → 204 |

The responses: `UserDto` { id, userName, email, roles } · `PizzaDto` { id, name, description, price, ingredients, createdDate, updatedDate } · `OrderDto` { id, addressTo, description, totalPrice, status, userId, userName, pizzas, createdDate, updatedDate }.

### 🖥️ A single-page app
```
MVC:              every click → the server builds a whole HTML page → the browser loads it again
Single-page app:  index.html + the JavaScript, once → every click: Angular swaps the view in the browser
                                                      → the data: JSON from our API, with the token
```
Our API never sends HTML: it sends data, and Angular builds the pages from it. That's what classes 6–10 are about.

---

<a id="homework"></a>
## 🏠 Homework
Check yourself with the last section of the `.http` file: every request must return the status code in its comment.

1️⃣ **One user:** `GET /api/users/{id}` → **200** + the user, with their roles
- **Admin only:** a customer → **403**
- an unknown id → **404**

2️⃣ **Change my password:** `PUT /api/users/me/password` with `currentPassword` and `newPassword` → **204**
- a wrong current password → **400**
- a new password that breaks Identity's rules → **400**, with Identity's messages in `errors`
- afterwards, logging in with the old password → **401**, with the new one → **200**
- tip: `UserManager`

3️⃣ **Delete my account:** `DELETE /api/users/me` → **204**
- my pizzas and orders are gone too
- logging in again → **401**

## ✅ By the end of the class
- 🙋 `GET /api/users/me` → 200 with id, userName, email and roles, never the password hash; `PUT /api/users/me` → 200, and a taken username → 400 with Identity's message
- 👑 the admin lists the users, sets a role (anything but `Admin` or `Customer` → 400) and deletes a user together with their orders; a customer on these endpoints → 403
- 🎟️ a new role counts from the user's next login
- 🌍 a request from `http://localhost:4200` gets the `Access-Control-Allow-Origin` header back
- 📄 the `.http` file returns the status code in every comment, up to the homework section
- 🧱 the solution builds with 0 warnings

---

🔜 **Next class:** Angular foundations: TypeScript for C# developers, components and templates, signals, routing. **Before it,** install Node.js 24 LTS, the Angular CLI 22 and VS Code with its three extensions: [Initial setup, 🅰️ For the frontend](../Class01/Initial-Setup.md). Keep your API: from class 8 on, the Angular app calls it.
