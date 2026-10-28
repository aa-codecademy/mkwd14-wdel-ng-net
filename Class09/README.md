# 🛡️ Class 09: Login, orders, guards and errors

> 🅰️ Frontend · class 4 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 8: the **Login page** as a typed **Reactive Form**, a header that follows the login, and an **interceptor** that sends the token. Then the cart becomes a real **order**: *Place order*, *My orders* with a *Cancel* button, **route guards** for logged-in users and for the admin, the admin's **orders board**, and one **error interceptor** that shows every API error to the user. 🔌 Start your API before the class, and keep `ana` / `Ana123!` at hand (the first request in `PizzaApp.Api.http`).

## 🗺️ What we do today

1. 🔁 Recap

**🧭 The end of class 8**

2. 📝 The `Login` page: a typed Reactive Form
3. 🧭 The header: what a logged-in user sees, and *Log out*
4. 🎟️ `tokenInterceptor`: the token on every request to our API

**📦 Orders**

5. 📦 Order models and `OrderService`
6. 🧾 Checkout: the delivery form and *Place order*
7. 📋 *My orders*: state in a service, and an `OrderCard`
8. ❌ Cancel a Pending order: `<ng-content>` and `changeStatus()`

**🛡️ Who sees what**

9. 🛡️ `authGuard`: log in first, then come back
10. 👨‍🍳 `adminGuard` and the admin's orders board

**🧯 Errors**

11. 🧯 `errorInterceptor`: every API error in one place

🏠 Three exercises are the [homework](#homework), and the first one is the *Register* page.

---

## 💡 Used concepts