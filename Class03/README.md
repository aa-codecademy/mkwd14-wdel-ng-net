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
9. 🍕 `PizzaService`: CRUD, only for the owner
10. 🎮 `PizzasController`: 201, 204 and `CancellationToken`
11. 🧪 Test with Scalar and the `.http` file (handout), the error cases last: 400, 403, 404

---

## 💡 Used concepts
