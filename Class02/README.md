# 🔑 Class 02: Authentication

> ⚙️ Backend · class 2 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md) · 🗄️ [Database diagram](../Class01/pizza-app-db-diagram.png)

Users **register** and **log in**, and get a **JWT** back. Every error, from any layer, comes back in **one JSON shape**.

## 🗺️ What we do today

1. 🔁 Recap
2. 🎟️ What a JWT is
3. ⚙️ `JwtSettings`: the `Jwt` section in `appsettings.json`, the secret key in user-secrets
4. 🖊️ `TokenService`: create and sign a token
5. 🛡️ Check the token on every request, and secure by default
6. 📨 The auth DTOs, with validation attributes
7. 🔑 `AuthService`: register and log in with `UserManager`
8. 🎮 `AuthController`, `AddApi()` and the new `Program.cs`
9. 📄 Handout: OpenAPI + Scalar, the API docs in the browser
10. 🧯 Our own exceptions and the `ErrorResponse` body
11. 🚨 `GlobalExceptionHandler`: one place that turns exceptions into HTTP responses
---

## 💡 Used concepts
