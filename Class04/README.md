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
