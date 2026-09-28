# 🍕 Full-stack development with Angular, .NET and PostgreSQL

Class materials and code. Over ten sessions we build a pizza ordering app, starting from an empty folder:
- a secure **REST API** in ASP.NET Core (.NET 10) that stores its data in **PostgreSQL**,
- an **Angular 22** app that uses it.

## 📖 About the subject

- 🗓️ **10 sessions** of 2h45 each (17:30–20:45, with two 15-minute breaks).
- ⚙️ **Sessions 1–5** build the backend: the API and the database.
- 🅰️ **Sessions 6–10** build the frontend: the Angular app on top of that API.
- 🎒 **You should already know** C#, OOP, SQL, ASP.NET and REST, plus HTML, CSS and JavaScript. 
- 🏁 **By the end** you'll have built both halves of a real web app, and connected them: database, API, security and user interface.

## 🎯 What we build

**Customers** can:
- register and log in,
- build their own pizza (size, ingredients, name) or start from one on the menu,
- save pizzas for later and collect pizzas in a cart,
- place an order, follow its status, and cancel it while it's still pending.

**Admins** can:
- see every order and move it along: *Pending → Preparing → Delivered* (or *Cancelled*),
- manage users and their roles.

```
┌────────────────────┐   HTTP + JSON    ┌─────────────────────────┐   EF Core   ┌──────────────┐
│  Angular 22 app    │ ───────────────► │  ASP.NET Core API       │ ──────────► │  PostgreSQL  │
│  localhost:4200    │ ◄─────────────── │  localhost:5004         │ ◄────────── │  PizzaAppDb  │
│  (in the browser)  │ JWT in a header  │  .NET 10                │             │              │
└────────────────────┘                  └─────────────────────────┘             └──────────────┘
    sessions 6–10                              sessions 1–5
```

## 🧰 Tech stack

| | Technology | What it does for us |
|---|---|---|
| 🗄️ **Database** | PostgreSQL 18 + pgAdmin 4 | a free, production-grade relational database, and a UI to look inside it |
| ⚙️ **Backend** | .NET 10 (LTS) and ASP.NET Core Web API | the REST API |
| | EF Core 10 + Npgsql | reads and writes PostgreSQL from C#, and creates the tables through migrations |
| | ASP.NET Core Identity + JWT | users, hashed passwords, roles and login tokens |
| | Mapster | copies data between entities and DTOs |
| | OpenAPI + Scalar | API documentation you can try out in the browser |
| 🅰️ **Frontend** | Angular 22 + TypeScript | the single-page app |
| | Angular Material 22 | ready-made UI components (buttons, cards, forms, snackbars) |
| | RxJS | HTTP calls as Observables |
| 🛠️ **Tools** | Visual Studio 2026 · VS Code · Node.js 24 LTS | the editors and the JavaScript runtime |

## 🗓️ Course plan

### ⚙️ Backend (.NET 10)

| # | Session | What we cover | You leave with |
|---|---|---|---|
| 1 | **Foundations and data** | the solution and its layers, `Directory.Build.props`, `.editorconfig`, entities with a `BaseEntity`, `DbContext` and entity configurations, user-secrets, the first migration, pgAdmin | a database whose tables were created from C# classes |
| 2 | **Authentication** | ASP.NET Core Identity, JWT (`TokenService`), the options pattern, custom exceptions and one global exception handler, secure by default | register, log in and get a token back |
| 3 | **Pizzas: the CRUD pattern** | a generic repository, DTOs with validation, Mapster, the current user, REST status codes (201, 204) | a complete API for saved pizzas, where each user sees only their own |
| 4 | **Orders, roles and business rules** | `Include`, a server-side total, the order status workflow, roles and the seeded admin, `[Authorize(Roles)]` | orders whose status only the right people can change |
| 5 | **Users, admin and wrap-up** | `/users/me`, admin user management, CORS for Angular, SOLID, DRY and KISS in the codebase | the finished API, ready for the Angular app |

### 🅰️ Frontend (Angular 22)

| # | Session | What we cover | You leave with |
|---|---|---|---|
| 6 | **Angular foundations** | a TypeScript crash course, components and templates, `@if` / `@for`, signals, routing, lazy loading | the app shell with the menu page |
| 7 | **Services and component communication** | `@Service()` and `inject()`, a cart service, `input()` / `output()` / `model()`, lifecycle, `effect()`, custom pipes and directives | the pizza maker and a working cart |
| 8 | **HTTP, forms and login** | environments, Observables, typed Reactive Forms with validation, the auth service, a token interceptor | register and log in from the browser |
| 9 | **Guards, errors and orders** | route guards, one error interceptor, placing orders, state in services, `<ng-content>`, the admin orders board | real orders from the checkout, and the admin board |
| 10 | **Saved pizzas, RxJS and wrap-up** | RxJS operators, a production build, code review of the whole app | the finished app and a production build |

## 🧭 How the sessions work

- 🔁 **Recap first.** Every session starts with a short recap of the previous one.
- ⌨️ **Live coding.** We write the code together, one layer at a time.
- 📄 **Handouts.** Some files (settings, styles, menu data) are pre-written. We open them, explain them and copy them in, instead of typing them.
- 🏠 **Homework.** Short exercises that repeat a pattern from class, such as one more endpoint or one more page. It's practice, not new material.
- 🆘 **Stuck?** Ask, in class or by email (see [Contact](#contact)).

## 📚 Class materials

Every class has its own folder: what we do in that class, and the concepts behind it.

## 🔗 Useful links
- Angular: https://angular.dev
- Angular Material: https://material.angular.dev
- RxJS: https://rxjs.dev/api
- ASP.NET Core: https://learn.microsoft.com/aspnet/core
- EF Core: https://learn.microsoft.com/ef/core
- PostgreSQL: https://www.postgresql.org/docs/
- Decode a JWT: https://jwt.io

<a id="contact"></a>
## 📬 Contact

**Ilija Mitev**, trainer

- 📧 Email: ilija.mitev3@gmail.com