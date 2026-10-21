# 🛒 Class 07: The pizza maker and the cart

> 🅰️ Frontend · class 2 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 6: a **`PizzaCard`** that gets its pizza through an **input**, the **header links** and a **404 page**. Then the menu leads somewhere: a **pizza maker** to build your own pizza, a **cart** that the whole app shares through a **service**, and the **cart page**. The API is still off today: it comes back in class 8.

## 🗺️ What we do today

1. 🔁 Recap

**🧭 The end of class 6**

2. 🍕 `PizzaCard`: data from the parent with `input()`
3. 🔗 Links with `routerLink` and `routerLinkActive`, and a 404 page

**🍕 The pizza maker**

4. 🍕 The `PizzaMaker` page: name, size and ingredients as signals, the price as a `computed`
5. 🔁 `IngredientPicker`: two-way binding with `model()`
6. 📤 *Make it*: `output()` from the card, and `?pizza=…` in the URL
7. ⏱️ Lifecycle: the constructor vs `ngOnInit`, and the query parameter as an input

**🛒 The cart**

8. 🛒 `CartService`: a service with `@Service()` and `inject()`, and the cart badge
9. 💾 `effect()`: the cart survives a page reload
10. 🧾 The `Checkout` page: the cart, remove, the total

**✨ Display helpers**

11. 🔤 Pipes: the built-in `currency`, and our own `ingredientName`
12. 🌶️ A custom directive: `HotPizza`

🏠 The [homework](#homework): two more buttons.

---

## 💡 Used concepts
