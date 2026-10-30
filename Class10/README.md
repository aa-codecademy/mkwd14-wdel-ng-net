# 🏁 Class 10: Guards, errors, saved pizzas and the wrap-up

> 🅰️ Frontend · class 5 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 9: *Cancel* on a Pending order, **route guards** for logged-in users and for the admin, the admin's **orders board**, and one **error interceptor** that shows every API error to the user. Then **saved pizzas** with the same state pattern as the orders, a look back at **RxJS**, a **production build**, and one request followed through the whole stack, from the button to PostgreSQL and back. 🔌 Start your API first (the `https` profile), and keep `ana` / `Ana123!` and `admin` / `Admin123!` at hand.

## 🗺️ What we do today

1. 🔁 Recap

**🧭 The end of class 9**

2. ❌ Cancel a Pending order: `<ng-content>` and `changeStatus()`
3. 🛡️ `authGuard`: log in first, then come back
4. 👨‍🍳 `adminGuard` and the admin's orders board
5. 🧯 `errorInterceptor`: every API error in one place

**🍕 Saved pizzas**

6. 💾 `PizzaService` and *Save for later*
7. 🍕 *My pizzas*

**🔁 RxJS**

8. 🔁 The operators we used, and when to unsubscribe

**🚀 Wrap-up**

9. 🚀 The production build
10. 🔍 One request through the whole stack

🏠 Six exercises are the [homework](#homework): the three from class 9, and three new ones.

---

## 💡 Used concepts
