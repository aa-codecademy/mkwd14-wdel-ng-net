# 📦 Class 04: Orders, roles and business rules

> ⚙️ Backend · class 4 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md) · 🗄️ [Database diagram](../Class01/pizza-app-db-diagram.png)

Customers **place orders**, and the **server** computes the total. Then **who may do what**: roles, ownership, and an order status that only moves the right way.

## 🗺️ What we do today

1. 🔁 Recap: the homework (`EnsureOwnerOrAdmin`, create, update and delete a pizza), and enums as text

**📦 Placing orders**

2. 📚 `OrderRepository`: an order together with its pizzas and its user (`Include`)
3. 📨 The order DTOs, and a mapping that reaches into the user
4. 🧾 `OrderService`: place an order with a total that the server computes, and list your orders
5. 🎮 `OrdersController`: `GET`, `GET {id}` and `POST`

**🛡️ Who may do what**

6. 👑 Roles and ownership: `[Authorize(Roles = …)]` and `EnsureOwnerOrAdmin`
7. 🚦 The status workflow: `OrderStatusRules` and `PATCH /api/orders/{id}/status`
8. 🧪 Test as a customer and as the admin, the refused cases last: 403, 409

🏠 Changing and deleting an order are the [homework](#homework).

---

## 💡 Used concepts

### 🔒 The homework: one ownership rule, written once
An **extension method on an interface**: every `ICurrentUser` gets it, and "owner or admin" lives in one place.
```csharp
public static class CurrentUserExtensions
{
    public static void EnsureOwnerOrAdmin(this ICurrentUser currentUser, string ownerId)
    {
        if (!currentUser.IsAdmin && currentUser.Id != ownerId)
        {
            throw new ForbiddenException();
        }
    }
}
```
```csharp
var pizza = await FindPizzaAsync(id, cancellationToken);   // 404
_currentUser.EnsureOwnerOrAdmin(pizza.UserId);             // 403
```
Create, update and delete follow the same steps as reading: **find → check → change → save → map**.
- 👤 `CreateAsync` takes the owner from the token, never from the request.
- ✏️ `UpdateAsync` copies the request **onto** the tracked pizza (`_mapper.Map(request, pizza)`), so EF Core saves only what changed.
- 🧾 Update and delete share one check: a pizza that is part of an order can't change → **409**. Today we finally have orders to try it.

### 🔤 Enums as text
Check your `AddApi()`: it needs the [`JsonStringEnumConverter` from class 3](../Class03/README.md). If it isn't there yet, add it now. Without it, `"ingredients": ["TomatoSauce"]` is a 400, and today's order status would travel as a number instead of `"status": "Pending"`.

---

### 📚 Include: related data, on request
EF Core loads only the entity's **own** columns. `order.Pizzas` and `order.User` stay empty unless you ask for them, and nothing warns you:
```csharp
IQueryable<Order> query = DbSet
    .AsNoTracking()
    .Include(order => order.Pizzas)   // a JOIN to Pizzas
    .Include(order => order.User);    // a JOIN to AspNetUsers

if (userId is not null)               // null = all orders (the admin)
{
    query = query.Where(order => order.UserId == userId);
}

return query
    .OrderByDescending(order => order.CreatedDate)
    .ToListAsync(cancellationToken);
```
Two queries, two jobs:

| Method | Tracking | Why |
|---|---|---|
| `GetWithDetailsAsync(userId)` | `AsNoTracking()` | the list: only read |
| `GetByIdWithDetailsAsync(id)` | tracked | one order that we often change and save right after (its status) |

### 📨 The order DTOs

| DTO | For | Fields |
|---|---|---|
| `AddOrderDto` | the `POST` body | address, description, pizzas (each one an `AddPizzaDto`) |
| `OrderDto` | every response | + `id`, `totalPrice`, `status`, `userId`, `userName`, the pizzas as `PizzaDto`, the dates |
| `ChangeOrderStatusDto` | the `PATCH` body | `status` |

- 💰 **No price and no total in `AddOrderDto`.** The server computes it.
- ✅ **Nested validation:** `[ApiController]` also checks every pizza **inside** the order. One pizza with an empty name → 400, with its message in `errors`.
- ❓ **Why `OrderStatus?` with `[Required]`?** A missing JSON field gets the type's default value. For an enum that is a number, so `[Required]` can't tell "missing" from "sent". With `?`, missing means `null`, and `[Required]` answers 400 *"The Status field is required."*

### 🔄 Mapster: a property from a related entity
Properties with the same name copy by themselves. `OrderDto.UserName` lives on the related `User`, so we say where it comes from:
```csharp
config.NewConfig<Order, OrderDto>()
    .Map(dest => dest.UserName, src => src.User != null ? src.User.UserName : null);

config.NewConfig<AddOrderDto, Order>();
```
The pizzas inside need nothing new: `AddPizzaDto → Pizza` and `Pizza → PizzaDto` from class 3 are reused for every item of the list.

### 🧾 OrderService: a total the server computes
```csharp
public async Task<OrderDto> CreateAsync(AddOrderDto request, CancellationToken cancellationToken = default)
{
    var order = _mapper.Map<Order>(request);
    order.UserId = _currentUser.Id;
    order.Status = OrderStatus.Pending;

    foreach (var pizza in order.Pizzas)
    {
        pizza.UserId = _currentUser.Id;
    }

    // Never trust a total sent by the client: calculate it on the server
    order.TotalPrice = order.Pizzas.Sum(pizza => pizza.Price);

    await _orderRepository.AddAsync(order, cancellationToken);

    return await GetByIdAsync(order.Id, cancellationToken);
}
```
- 💰 **Money is computed on the server.** A client that sends `"totalPrice": 0.01` changes nothing: the field isn't in the DTO, so it's ignored.
- 🔗 **One `AddAsync`, one transaction:** EF Core inserts the order, then its pizzas with the new `OrderId`. Either everything is saved or nothing is.
- 🔁 **Read it back:** after saving, only `UserId` is set, not `User`. `GetByIdAsync` loads the order again with its user, so the response has the `userName` too.
- 🤔 **And the pizza prices?** They still come from the client. A real shop would keep a price list on the server; we leave that out.

### 🎮 OrdersController

| Request | Success |
|---|---|
| `GET /api/orders` | **200** + your orders, newest first (the admin: all of them) |
| `GET /api/orders/{id}` | **200** + the order |
| `POST /api/orders` | **201 Created** + the order + a `Location` header |
| `PATCH /api/orders/{id}/status` | **200** + the order with its new status |
| 🏠 `PUT /api/orders/{id}` | **200** + the updated order |
| 🏠 `DELETE /api/orders/{id}` | **204 No Content** |

🩹 **PATCH or PUT?** `PUT` sends every field you may edit, and they all get replaced. `PATCH` changes **part** of a resource: here only its status, at its own route `/{id}/status`.

---

### 👑 Roles and ownership: three levels of "may I?"

| Level | Where | Answered by | Refused |
|---|---|---|---|
| 🔐 logged in | the fallback policy (class 2), on every endpoint | the authentication middleware, before the controller | **401** |
| 👑 a role | `[Authorize(Roles = Roles.Admin)]` on an action or a controller | the authorization middleware, before the controller | **403** |
| 🔒 the owner | `EnsureOwnerOrAdmin` in the service | our `GlobalExceptionHandler` | **403** + `ErrorResponse` |

- 🎟️ **The role comes from the token:** the `role` claim from class 2. A JWT is a snapshot, so a role changed in the database counts from the user's **next login**.
- 🔤 **Why `Roles.Admin` and not `"Admin"`?** Only constants can go into an attribute, and `Roles` holds `const string`s. A typo in `"Admn"` compiles and quietly locks everybody out; a typo in `Roles.Admn` doesn't compile.
- 🫙 **The middleware answers without a body:** a 401 or a 403 from the first two levels has no `ErrorResponse`, because no exception was thrown. A client checks the **status code** first.
- ♻️ **`EnsureOwnerOrAdmin` guards orders too:** reading an order and changing its status call it, like the pizzas do. One rule, two services: DRY.

### 🚦 The status workflow
An order moves only along the arrows. **Delivered** and **Cancelled** are final.
```
 Pending ───admin───► Preparing ───admin───► Delivered
    │                     │
    │ admin, or the       │ admin
    │ customer (own)      │
    ▼                     │
 Cancelled ◄──────────────┘
```
The rule lives in **one** small class, with no database and no HTTP: a pure function that's easy to read and to test

`ChangeStatusAsync` follows the CRUD steps: **find** (404) → **owner or admin** (403) → **the rule** (409) → change → save → map.

### 🔢 400, 403 or 409?

| | Means | Example |
|---|---|---|
| **400** Bad Request | the request itself is wrong | `"status": "Cooking"`, or no status at all |
| **403** Forbidden | not yours, or not your role | Bob changes Ana's order |
| **409** Conflict | a valid request, but not in the current state | cancel a **Delivered** order; change a pizza that is in an order |

### 🧪 Test
📄 The `.http` file gets the orders. Run it as the customer **and** as the admin (`admin` / `Admin123!`):
```http
### Place an order → 201: the server computes totalPrice (7.50 + 9.00 = 16.50), status Pending
# @name createOrder
POST {{baseUrl}}/orders
Authorization: Bearer {{customerLogin.response.body.$.token}}
Content-Type: application/json

{
  "addressTo": "Main Street 10",
  "pizzas": [
    { "name": "Margherita", "price": 7.50, "ingredients": ["TomatoSauce", "Mozzarella", "Basil"] },
    { "name": "Pepperoni", "price": 9.00, "ingredients": ["TomatoSauce", "Mozzarella", "Pepperoni"] }
  ]
}

### The admin moves it along → 200
PATCH {{baseUrl}}/orders/{{createOrder.response.body.$.id}}/status
Authorization: Bearer {{adminLogin.response.body.$.token}}
Content-Type: application/json

{ "status": "Preparing" }
```
🔴 The refused cases last: Bob reads Ana's order (403), a pizza inside the order is changed (409), a Delivered order is cancelled (409).

---

<a id="homework"></a>
## 🏠 Homework
We go through it at the start of class 5.

1️⃣ **Change an order:** `PUT /api/orders/{id}` → **200** + the updated order
- only the delivery address and the description can change, with the same rules as when the order was placed
- only its owner or an admin: anyone else → **403**, an unknown id → **404**
- only while the order is **Pending**: otherwise → **409**

2️⃣ **Delete an order:** `DELETE /api/orders/{id}` → **204**
- **Admin only:** a customer → **403**
- an unknown id → **404**
- its pizzas are gone too

## ✅ By the end of the class
- 📦 `POST /api/orders` → 201 with the server's total, status Pending, and the customer's `userName`
- 📋 `GET /api/orders`: a customer gets their own orders, the admin everybody's; `GET /api/orders/{id}`: the owner or an admin, anyone else → 403
- 🚦 the admin moves an order Pending → Preparing → Delivered; a customer cancels their own Pending order; any other move → 409
- 🧾 a pizza inside an order can't be changed or deleted → 409
- 🧱 the solution builds with 0 warnings

---

🔜 **Next class:** Users, admin and wrap-up. We start with the homework, then `/users/me`, admin user management, CORS for the Angular app, and a look back at SOLID, DRY and KISS in our code. Introduction to Angular.
