# 🔑 Class 02: Authentication

> ⚙️ Backend · class 2 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md) · 🗄️ [Database diagram](../Class01/pizza-app-db-diagram.png)

Users **register** and **log in**, and get a **JWT** back. Every error, from any layer, comes back in **one JSON shape**.

## 🗺️ What we do today

1. 🔁 Recap

**🎟️ The token**

2. 🎟️ What a JWT is
3. ⚙️ `JwtSettings`: the `Jwt` section in `appsettings.json`, the secret key in user-secrets
4. 🖊️ `TokenService`: create and sign a token
5. 🛡️ Check the token on every request, and secure by default

**🔑 Register and log in**

6. 📨 The auth DTOs, with validation attributes
7. 🔑 `AuthService`: register and log in with `UserManager`
8. 🎮 `AuthController`, `AddApi()` and the new `Program.cs`
9. 📄 Handout: OpenAPI + Scalar, the API docs in the browser

**🧯 Errors**

10. 🧯 Our own exceptions and the `ErrorResponse` body
11. 🚨 `GlobalExceptionHandler`: one place that turns exceptions into HTTP responses

---

## 💡 Used concepts

### 🎟️ JWT (JSON Web Token)
After a successful login the API gives the client a **token**. The client sends it with **every** request, and the API checks it instead of asking for the password again:
```
POST /api/auth/login  { userName, password }   →   200 { token, expiresAt, userName, roles }
GET  /api/pizzas      Authorization: Bearer eyJhbGciOi…   →   the API checks the token
```
A JWT has three parts, separated by dots: **header.payload.signature**.
- 📋 The **payload** holds the **claims**: facts about the user. It's only Base64, **not encrypted**: paste a token into [jwt.io](https://jwt.io) and anybody can read it. Never put a password or a secret in it.
- ✍️ The **signature** is made with our **secret key**. Change one character of the payload and the signature no longer matches, so the API refuses the token.

| Claim | Meaning | Ours |
|---|---|---|
| `sub` | subject: who the token is about | the user's `Id` |
| `name`, `email` | the user's name and email | `ana`, `ana@pizzaapp.local` |
| `role` | one claim per role | `Customer` |
| `jti` | a unique id for this token | a new `Guid` |
| `iss`, `aud` | who issued it, who it's meant for | `PizzaApp.Api`, `PizzaApp.Client` |
| `exp` | when it stops working | 60 minutes after the login |

🤔 **Why only 60 minutes?** The server keeps no session, so it can't "log out" a token: a stolen token works until it expires. Short-lived tokens limit the damage.

### ⚙️ The options pattern + ValidateOnStart
The JWT settings live in a section of `appsettings.json`. The secret key stays **empty** there:
```json
"Jwt": {
  "Issuer": "PizzaApp.Api",
  "Audience": "PizzaApp.Client",
  "SecretKey": "",
  "ExpirationInMinutes": 60
}
```
The real key goes into **user-secrets** (class 1), next to the connection string. Make your own random key, 64 characters long:
```powershell
$bytes = New-Object byte[] 48; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
dotnet user-secrets set "Jwt:SecretKey" ([Convert]::ToBase64String($bytes)) --project PizzaApp.Api
```
A **settings class** describes the section, with the same validation attributes as a DTO:
```csharp
public class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(32)]   // HMAC-SHA256 needs at least 256 bits
    public string SecretKey { get; set; } = string.Empty;

    // … Issuer, Audience, ExpirationInMinutes
}
```
One registration binds the section to the class and checks it:
```csharp
services.AddOptions<JwtSettings>()
    .BindConfiguration(JwtSettings.SectionName)   // the "Jwt" section → JwtSettings
    .ValidateDataAnnotations()                    // check [Required], [MinLength], …
    .ValidateOnStart();                           // … when the app starts, not on the first login
```
🚦 **Fail fast:** no key, or one shorter than 32 characters, and the API **refuses to start** with an `OptionsValidationException` that names the setting. Better than a crash on the first login.

### 🖊️ TokenService: create and sign a token
A class that needs the settings asks for `IOptions<JwtSettings>`:
```csharp
public TokenService(IOptions<JwtSettings> jwtSettings)
{
    _jwtSettings = jwtSettings.Value;
}
```
`CreateToken` collects the claims, then signs them with the key:
```csharp
var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub, user.Id),
    new(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
    new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};
claims.AddRange(roles.Select(role => new Claim(JwtSettings.RoleClaimType, role)));

var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(claims),
    Issuer = _jwtSettings.Issuer,
    Audience = _jwtSettings.Audience,
    Expires = expiresAt,
    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
};

var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);
```

### 🛡️ JWT bearer authentication: check the token
On every request, the JwtBearer middleware reads the `Authorization: Bearer …` header and checks the token with the **same** settings that created it:
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// then, in AddJwtAuthentication(): options = the JwtBearer options, jwt = the validated JwtSettings
options.MapInboundClaims = false;   // keep the claim names as they are in the token: "sub", "name", "role"
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = jwt.Issuer,
    ValidateAudience = true,
    ValidAudience = jwt.Audience,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromMinutes(1),   // the default allows 5 minutes after "exp"
    NameClaimType = JwtRegisteredClaimNames.Name,
    RoleClaimType = JwtSettings.RoleClaimType,
};
```
A valid token becomes `User` (a `ClaimsPrincipal`) in every controller: `User.Identity.Name` is the `name` claim.

⛓️ **The middleware order matters:**
```csharp
app.UseAuthentication();   // who are you?   (reads the token)
app.UseAuthorization();    // are you allowed? (needs the answer above)
```
| | Means | When |
|---|---|---|
| **401** Unauthorized | we don't know who you are | no token, an expired one, or a changed one |
| **403** Forbidden | we know you, but you may not do this | a customer on an admin endpoint (class 4) |

### 🔒 Secure by default: the fallback policy
Normally an endpoint is **public** until somebody remembers `[Authorize]`. We turn it around: every endpoint needs a logged-in user, unless it says `[AllowAnonymous]`:
```csharp
services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());
```
Forgetting an attribute now closes an endpoint by mistake, which we notice right away, instead of opening it, which we might never notice. Try it: `/openapi/v1.json` answers **401** now.

---

### 📨 The auth DTOs and their validation
`[ApiController]` checks the validation attributes **before** the action runs, and answers 400 by itself:
```csharp
public class RegisterRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    // …
}
```
`UserDto` is a **plain class**, not the `User` entity, so the password hash can never end up in a response.

### 🔑 Register and log in with UserManager
Identity's `UserManager<User>` (class 1) does the hard work. Today we use five of its methods:

| Method | What it does |
|---|---|
| `CreateAsync(user, password)` | checks the password rules and the unique username and email, **hashes** the password, saves the user |
| `AddToRoleAsync(user, Roles.Customer)` | a row in `AspNetUserRoles`: everybody who registers is a Customer |
| `FindByNameAsync(userName)` | the user, or `null` |
| `CheckPasswordAsync(user, password)` | hashes the password again and compares the hashes |
| `GetRolesAsync(user)` | the role names, for the token |

🔒 **Identity's default password rules:** at least 6 characters, with an uppercase letter, a lowercase letter, a digit and a symbol. `CreateAsync` doesn't throw when a rule fails: it returns an `IdentityResult` with the reasons, and we throw them as a `BadRequestException` ([our own exceptions](#errors) are the last part of the class):
```csharp
var result = await _userManager.CreateAsync(user, request.Password);
if (!result.Succeeded)
{
    throw new BadRequestException("Registration failed.", result.Errors.Select(error => error.Description));
}
```
🕵️ **One message for both mistakes.** If "unknown user" and "wrong password" got different messages, anybody could use the login form to find out which usernames exist (*user enumeration*):
```csharp
var user = await _userManager.FindByNameAsync(request.UserName);
if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
{
    throw new UnauthorizedException("Invalid username or password.");
}
```

### 🎮 ApiControllerBase
The attributes every controller needs, written once:
```csharp
[ApiController]
[Route("api/[controller]")]   // AuthController → api/auth
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
}
```
```csharp
[AllowAnonymous]   // the fallback policy closed everything: register and login must work without a token
public class AuthController : ApiControllerBase
```
`LowercaseUrls = true` in `AddApi()` makes the routes lowercase: `/api/auth/login`. `Register` answers **201 Created**, not 200: a new user exists now.

### 📄 OpenAPI + Scalar
.NET builds a description of the API by itself: http://localhost:5004/openapi/v1.json. **Scalar** turns it into a page where you can try every endpoint: http://localhost:5004/scalar.
- 📄 **Handout:** `OpenApiExtensions` (both pages are `AllowAnonymous`) and `BearerSecuritySchemeTransformer`, which adds an **Auth** field to Scalar. Paste the token from the login there (without the word `Bearer`), and Scalar sends it with every request.
- `launchSettings.json` opens `/scalar` when you press F5 (`"launchUrl": "scalar"`).

---

<a id="errors"></a>
### 🧯 Errors are exceptions
A service that finds a problem simply **throws**. It doesn't know about HTTP status codes, and there's **no try/catch** anywhere in the services or the controllers. Our exceptions live in `PizzaApp.Shared`:

| Exception | HTTP | Example |
|---|---|---|
| `BadRequestException` | 400 | a weak password (+ the list of reasons in `Errors`) |
| `UnauthorizedException` | 401 | a wrong username or password |
| `ForbiddenException` | 403 | changing another user's pizza (class 3) |
| `NotFoundException` | 404 | an unknown id (class 3) |
| `ConflictException` | 409 | a status change that isn't allowed (class 4) |
| anything else | 500 | a bug |

They all inherit `AppException`, an `abstract` class: nobody throws a plain `AppException`.

---

## 🏠 Homework
None today. Make sure register and login work on your computer: class 3 builds on them.

## ✅ By the end of the class
- 🎟️ `POST /api/auth/register` → 201 with the new user and `"roles": ["Customer"]`
- 🔑 `POST /api/auth/login` → 200 with a token; jwt.io shows its claims. The seeded `admin` / `Admin123!` gets `"roles": ["Admin"]`
- 🚦 without `Jwt:SecretKey` the API refuses to start
- 📄 Scalar at http://localhost:5004/scalar
- 🧯 a wrong password → 401, a weak password → 400 with Identity's messages, an empty form → 400, all in the `ErrorResponse` shape
- 🧱 the solution builds with 0 warnings

---

🔜 **Next class:** Pizzas, the CRUD pattern. A generic repository, Mapster, the current user, and the first endpoints that need a token.
