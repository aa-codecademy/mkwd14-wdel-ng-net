# 📝 Class 08: HTTP, login and forms

> 🅰️ Frontend · class 3 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 7: a **cart** that the whole app shares through a **service**, the **cart page**, and two display helpers, a **pipe** and a **directive**. Then the app talks to our **API** for the first time: **Observables**, an **`AuthService`** that remembers who is logged in, an **interceptor** that sends the token, and typed **Reactive Forms** for the login and register pages. 🔌 Start your API before the class, with the **`http`** profile.

## 🗺️ What we do today

1. 🔁 Recap

**🧭 The end of class 7**

2. 🛒 `CartService`: a service with `@Service()` and `inject()`, and the cart badge
3. 💾 `effect()`: the cart survives a page reload
4. 🧾 The `Checkout` page: the cart, remove, the total
5. 🔤 Pipes: the built-in `currency`, and our own `ingredientName`
6. 🌶️ A custom directive: `HotPizza`

**🌐 Talking to the API**

7. ⚙️ Environments: the API's address in one place, and models that mirror the DTOs
8. 📡 `HttpClient` and Observables: the first request

**🎟️ The session and the token**

9. 🔐 `AuthService`: the login in a signal and in `localStorage`
10. 🎟️ `tokenInterceptor`: the token on every request to our API

**📝 The forms**

11. 📝 The `Login` page: a typed Reactive Form
12. 🆕 The `Register` page: validators that mirror the DTO, and a snackbar
13. 🧭 The header: what a logged-in user sees, and *Log out*

🏠 Three small exercises are the [homework](#homework).

---

## 💡 Used concepts
