# 🧱 Class 01: Foundations and data

> ⚙️ Backend · class 1 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](Initial-Setup.md) · 🗄️ [Database diagram](database-diagram.png)

From an **empty folder** to a **PostgreSQL database whose tables are created from our C# classes**.

## 🗺️ What we do today

1. 🏛️ The app, its layers and the seven projects
2. 🧩 Create the solution (`PizzaApp.slnx`, the new solution format), the projects and their references
3. 📄 Handout: the shared settings files (`global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `dotnet-tools.json`)
4. 🍕 The domain: `BaseEntity`, `Pizza`, `Order`, `User`, and the enums
5. 🗄️ `PizzaAppDbContext` and one configuration class per entity
6. 💉 `AddDataAccess()`: register the `DbContext`
7. 🔐 The connection string in user-secrets
8. 🐘 The first migration, then a look at the tables in pgAdmin

---

## 💡 New concepts

### 📦 Directory.Packages.props
**Central Package Management:** every NuGet **version** is defined once, here. A project only says **which** package it uses.
```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.3" />

<!-- PizzaApp.DataAccess.csproj: no version -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
```
⚠️ The webapi template puts a `Version="…"` on its `Microsoft.AspNetCore.OpenApi` reference. Delete it, or the build fails with **`NU1008`**.

---

### ✏️ .editorconfig
One file with the **code style** for the whole solution: indentation, `using` order, braces, naming rules. Visual Studio, VS Code and Rider read it, and the build checks it too.
```ini
[*.cs]
csharp_style_namespace_declarations = file_scoped:warning   # namespace X;
csharp_prefer_braces = true:warning                          # always { } after if/for/...
```
Each rule has a **severity**: `none` · `silent` · `suggestion` (grey dots) · `warning` (green squiggle) · `error` (the build fails).
Our naming rules: interfaces `IPizzaService`, types and members `PascalCase`, private fields `_camelCase`, parameters and locals `camelCase`.
🔧 **Ctrl + .** fixes a warning; `dotnet format` fixes the whole solution.

---

### 🪪 ASP.NET Core Identity
Microsoft's ready-made **membership system**: all the user and password code we should **never** write ourselves.
- 👤 **Users:** create, find, update, delete
- 🔑 **Passwords:** saved only as a salted **hash**, never as plain text, and checked against password rules (length, digits, …)
- 🛡️ **Roles and claims:** `Admin`, `Customer`, …
- 🔒 **Extras:** lockout after too many wrong passwords, email confirmation, two-factor login

Identity has three layers:

| Layer | What it does | Classes |
|---|---|---|
| Entities | the data | `IdentityUser`, `IdentityRole` |
| Stores | save it to the database with EF Core | `IdentityDbContext<User>` |
| Managers | the methods **we** call | `UserManager<User>`, `SignInManager<User>`, `RoleManager<IdentityRole>` |

```csharp
var result = await _userManager.CreateAsync(user, "Pizza123!");      // hashes the password, saves the user
bool isValid = await _userManager.CheckPasswordAsync(user, "Pizza123!");
```
⚠️ Identity checks **who** the user is, but it doesn't create the JWT. In class 2, our own code creates the token after the password check succeeds.

### 👤 IdentityUser
ASP.NET Core Identity's user class. It already has `Id`, `UserName`, `Email`, `PasswordHash`, … so our `User` only adds its relationships:
```csharp
public class User : IdentityUser
{
    public List<Order> Orders { get; set; } = [];
    public List<Pizza> Pizzas { get; set; } = [];
}
```
🤔 `Pizza` and `Order` inherit `BaseEntity` (`Id`, `CreatedDate`, `UpdatedDate`), but `User` can't: a C# class has only **one** base class.

### 🗄️ IdentityDbContext\<User\>
Our `DbContext` inherits Identity's version instead of the plain `DbContext`:
```csharp
public class PizzaAppDbContext : IdentityDbContext<User>
```
So the migration also creates Identity's tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, …), ready for registration and login in class 2.

---

### ⚙️ Configuration classes + ApplyConfigurationsFromAssembly
Instead of configuring every table inside `OnModelCreating`, each entity gets its own class that implements `IEntityTypeConfiguration<T>`:
```csharp
public class PizzaConfiguration : IEntityTypeConfiguration<Pizza>
{
    public void Configure(EntityTypeBuilder<Pizza> builder)
    {
        builder.HasKey(pizza => pizza.Id);
        builder.Property(pizza => pizza.Name).IsRequired().HasMaxLength(100);
        builder.Property(pizza => pizza.Price).HasPrecision(10, 2);   // numeric(10,2)
    }
}
```
One line in the `DbContext` finds and applies **every** configuration class in the project:
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);   // Identity's tables first
    builder.ApplyConfigurationsFromAssembly(typeof(PizzaAppDbContext).Assembly);
}
```
A new entity only needs a new configuration class: the `DbContext` doesn't change.

---

### 🔐 User-secrets
Passwords and keys must **never** reach GitHub. `appsettings.json` keeps an empty placeholder, and the real value lives in **user-secrets**: a file **outside the project folder** (`%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`).
- **Visual Studio:** right-click **PizzaApp.Api → Manage User Secrets**:
  ```json
  {
    "ConnectionStrings:PizzaAppDb": "Host=localhost;Port=5432;Database=PizzaAppDb;Username=postgres;Password=<your postgres password>"
  }
  ```
- **Terminal** (in the `PizzaApp` folder):
  ```powershell
  dotnet user-secrets set "ConnectionStrings:PizzaAppDb" "Host=localhost;Port=5432;Database=PizzaAppDb;Username=postgres;Password=<your postgres password>" --project PizzaApp.Api
  ```
In Development, user-secrets **override** `appsettings.json`. The `:` means "one level deeper": `ConnectionStrings:PizzaAppDb`.

---

🔜 **Next class:** Authentication, with Identity, JWT and one global exception handler.
