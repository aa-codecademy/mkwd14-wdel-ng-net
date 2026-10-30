# 🏁 Class 10: Guards, errors, saved pizzas and the wrap-up

> 🅰️ Frontend · class 5 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 9: _Cancel_ on a Pending order, **route guards** for logged-in users and for the admin, the admin's **orders board**, and one **error interceptor** that shows every API error to the user. Then **saved pizzas** with the same state pattern as the orders, a look back at **RxJS**, a **production build**, and one request followed through the whole stack, from the button to PostgreSQL and back. 🔌 Start your API first (the `https` profile), and keep `ana` / `Ana123!` and `admin` / `Admin123!` at hand.

## 🗺️ What we do today

1. 🔁 Recap

**🧭 The end of class 9**

2. ❌ Cancel a Pending order: `<ng-content>` and `changeStatus()`
3. 🛡️ `authGuard`: log in first, then come back
4. 👨‍🍳 `adminGuard` and the admin's orders board
5. 🧯 `errorInterceptor`: every API error in one place

**🍕 Saved pizzas**

6. 💾 `PizzaService` and _Save for later_
7. 🍕 _My pizzas_

**🔁 RxJS**

8. 🔁 The operators we used, and when to unsubscribe

**🚀 Wrap-up**

9. 🚀 The production build
10. 🔍 One request through the whole stack

🏠 Six exercises are the [homework](#homework): the three from class 9, and three new ones.

---

## 💡 Used concepts

### 🧩 `<ng-content>`: a card with a slot for buttons

```html
<!-- order-card.html: the card shows the order, and leaves room for buttons -->
<mat-card-actions align="end">
  <ng-content />
</mat-card-actions>
```

```html
<!-- my-orders.html: everything between the tags lands where <ng-content /> is -->
<app-order-card [order]="order">
  @if (order.status === 'Pending') {
  <button matButton type="button" (click)="orderService.changeStatus(order.id, 'Cancelled')">
    Cancel order
  </button>
  }
</app-order-card>
```

- 🧱 **The card stays simple:** it shows an order and knows nothing about buttons or the API. _My orders_ puts _Cancel_ into it, the admin page puts _Start preparing_ into the same card.
- 🔀 **Inputs for data, `<ng-content>` for markup:** `[order]` gives the card its data, the content between the tags gives it HTML.
- 🔁 **`changeStatus()` replaces one order** in the service's signal with the one the API sent back (`update` + the array's `map`), so the page follows without loading the list again.

### 🔀 `@switch`

```html
@switch (order.status) { @case ('Pending') {
<!-- Start preparing, Cancel -->
} @case ('Preparing') {
<!-- Mark delivered, Cancel -->
} @case ('Delivered') {
<!-- Mark returned -->
} }
```

- 🔁 **C#'s `switch`, in the template:** one block per value, no `break`. A status without a `@case` (Cancelled, Returned) shows nothing.
- ⚖️ **The buttons mirror `OrderStatusRules` on the backend,** and the API still decides: a change it refuses comes back as a 409.

### 🛡️ Guards

```ts
export const authGuard: CanActivateFn = (_route, state) => {
  if (inject(AuthService).isLoggedIn()) {
    return true;
  }

  return inject(Router).createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
```

```ts
// app.routes.ts
{ path: 'orders', canActivate: [authGuard], loadComponent: … },
{
  path: 'admin',
  canActivate: [authGuard, adminGuard], // first "logged in?", then "admin?"
  children: [{ path: 'orders', loadComponent: … }],
},
```

- 🚪 **A guard runs before the page opens** and answers `true` (come in) or a `UrlTree` (go there instead). It never calls `navigate()` itself.
- ↩️ **`returnUrl`:** the guard remembers where you wanted to go (`/login?returnUrl=%2Forders`), and the Login page reads it as an input (like `?pizza=` in class 7), so after the login you land there.
- 👮 **`adminGuard`** checks `isAdmin()` and sends everyone else to the menu.
- 🔐 **A guard is for the user, not for security:** anyone can change the code in their browser. The API checks the token and the role on every request anyway, like `[Authorize(Roles = Roles.Admin)]`.
- 🚪 **Guards run only when you navigate:** that's why _Log out_ opens the login page itself, instead of leaving you on a protected page.

### 🧯 One error interceptor for the whole app

```ts
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);
  …
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        notificationService.error("Can't reach the server. Is the API running?");
      } else if (error.status === 401 && authService.isLoggedIn()) {
        authService.logout(); // the token expired
        notificationService.error('Your session has expired. Please log in again.');
        router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
      } else {
        notificationService.error(toMessage(error)); // the API's errors[], or its message
      }

      return EMPTY; // finish quietly: no value, no error
    }),
  );
};
```

```ts
// app.config.ts: the interceptors run in this order
provideHttpClient(withInterceptors([tokenInterceptor, errorInterceptor])),
```

- 🎯 **One place, like `GlobalExceptionHandler` on the backend:** no page needs an `error` callback.
- 📦 **`ErrorResponse`** is the backend's error body: `{ statusCode, message, errors, traceId }`. `errors` is filled for validation and Identity; otherwise `message` says what went wrong.
- 0️⃣ **Status 0** means there was no answer at all: the API isn't running, the network is down, or CORS blocked it.
- 🔑 **401 with a session** means the token expired; a 401 without one is a wrong password, which gets its own message.
- 🤫 **`EMPTY`** ends the request without a value: the page's `subscribe` callback is skipped (the cart stays, the login page stays), and `finalize` still runs (the button comes back).

### 🍕 The same pattern, a third time

`PizzaService` keeps the saved pizzas exactly as `OrderService` keeps the orders: a private signal, `asReadonly()` for the pages, and a `load()` that fills it.

```ts
delete(id: number): void {
  this.http
    .delete<void>(`${this.url}/${id}`)
    .subscribe(() =>
      this.pizzasState.update((pizzas) => pizzas.filter((pizza) => pizza.id !== id)),
    );
}
```

| Method                                 | Who subscribes | Why                                                                             |
| -------------------------------------- | -------------- | ------------------------------------------------------------------------------- |
| `load()`, `delete()`, `changeStatus()` | the service    | the answer changes the shared list, and every page that reads it follows        |
| `create()`, `login()`                  | the page       | only the page knows what success means: a snackbar, an empty cart, another page |

- 🗃️ **Server data lives in a service, not in a page:** one list, one place that changes it.
- 💾 **_Save for later_** uses the busy-button pattern from Checkout (`saving` + `finalize`), and its errors need no code: the interceptor shows them.

### 🔁 RxJS: what we used

| Operator     | Where in our app                     | Does                                                                                                                  |
| ------------ | ------------------------------------ | --------------------------------------------------------------------------------------------------------------------- |
| `tap`        | `AuthService.login()`                | something on the side (save the session), the value passes on unchanged                                               |
| `map`        |                                      | turns each value into another one                                                                                     |
| `finalize`   | `load()`, Checkout, _Save for later_ | runs once at the end, after success or error: C#'s `finally`                                                          |
| `catchError` | `errorInterceptor`                   | replaces an error with another Observable (here `EMPTY`)                                                              |
| `switchMap`  |                                      | turns each value into a new request and **cancels the previous one**: search as you type, only the last answer counts |

```ts
constructor() {
  interval(1000)
    .pipe(takeUntilDestroyed()) // stops when the component is destroyed
    .subscribe((second) => console.log(second));
}
```

- 🛑 **Do I have to unsubscribe?** Not from `HttpClient`: a request completes after its answer. From streams that never complete (`interval`, a `Subject`, a form's `valueChanges`, the router's events): yes, with `takeUntilDestroyed()` from `@angular/core/rxjs-interop`.
- 📍 **`takeUntilDestroyed()` needs an injection context:** the constructor or a field. Anywhere else, pass it a `DestroyRef` (`inject(DestroyRef)`).

### 🆕 `httpResource`: Angular's newer way to load data

The _My pizzas_ list in one line, with the API that's stable since Angular 22. Newer projects and the docs use it, while most existing code uses `HttpClient` with Observables in a service, so we do too. Just learn to recognize it:

```ts
protected readonly pizzas = httpResource<PizzaDto[]>(() => `${environment.apiUrl}/pizzas`, {
  defaultValue: [],
});
```

```html
@if (pizzas.isLoading()) {
<mat-spinner />
} @else { @for (pizza of pizzas.value(); track pizza.id) { … } }
```

- 🚦 **Everything is a signal:** `value()`, `isLoading()`, `error()`. The request starts by itself, and runs again when a signal used in the URL function changes.

### 🚀 The production build

```powershell
ng build        # production (angular.json: "defaultConfiguration": "production")
npx serve -s dist/pizza-app/browser -l 4200   # try the built app; -s = single-page app
```

- 📦 **`ng serve` builds for you, `ng build` builds for users:** compiled templates, unused code left out (tree-shaking), minified, and every file name gets a hash (`main-ON3DZVKU.js`) that changes only when the file does, so browsers can cache it.
- 🧩 **Initial vs lazy chunks:** the initial files load before the first page; each lazy chunk is one page, loaded the first time someone opens it.
- 📏 **Budgets** in `angular.json`: over 600 kB initial, `ng build` warns; over 1 MB, it fails. A size test.
- 🔁 **Two environment files:** `environment.ts` holds the production values; `ng serve` swaps in `environment.development.ts` (`fileReplacements`). Never put a secret in either: every visitor downloads them.
- 🌍 **`dist/pizza-app/browser` is plain files:** any static host can serve them, with two rules. It must send `index.html` for every app URL (reload `/orders` without `-s` → 404), and the app's address must be in the API's `Cors:AllowedOrigins` (from another port, every request fails with status 0).

### 🗺️ One request through the whole stack

```text
🅰️ Save for later → PizzaService.create() → tokenInterceptor (+ Bearer token) → errorInterceptor → POST /api/pizzas
⚙️ UseExceptionHandler → UseCors → UseAuthentication (the JWT) → UseAuthorization
   → PizzasController (AddPizzaDto's rules, or 400) → PizzaService (UserId from the token) → PizzaRepository
🗄️ EF Core: INSERT INTO "Pizzas" … → the new Id
↩️ 201 + PizzaDto → errorInterceptor (no error) → the subscribe callback: "… is saved in My pizzas." → finalize
   an error anywhere on the way → GlobalExceptionHandler → ErrorResponse → errorInterceptor → a snackbar
```

### 🔁 The same ideas on both sides

| ⚙️ Backend                                | 🅰️ Frontend                                                 |
| ----------------------------------------- | ----------------------------------------------------------- |
| `Program.cs`: the composition root        | `app.config.ts`                                             |
| middleware, `await next()`                | interceptors, `next(req)`                                   |
| `GlobalExceptionHandler`                  | `errorInterceptor`                                          |
| `[Authorize]`, the fallback policy        | guards (for the user only: the API is the security)         |
| DTOs with `[Required]`, `[StringLength]`  | interfaces, and `Validators` that mirror the attributes     |
| services and `AddScoped` / `AddSingleton` | `@Service()` and `inject()`: one instance for the whole app |
| `appsettings.json` and user-secrets       | `environment.ts` (no secrets!)                              |

### 🔐 Where the token lives

- 🕵️ **`localStorage` is simple, and readable by any script on the page:** an attacker who gets JavaScript in (XSS) can read the token and use it until it expires.
- 🍪 **Safer: an httpOnly cookie,** set by the server, invisible to JavaScript, sent by the browser on its own. It needs `withCredentials`, `AllowCredentials()` in CORS and CSRF protection, so it's a good next step, not a first one.

### 🗂️ Where things go

```
src/app/
├── core/
│   ├── guards/        auth-guard.ts, admin-guard.ts
│   ├── interceptors/  token-interceptor.ts, error-interceptor.ts
│   ├── layout/        header/, not-found/
│   └── services/      auth-service.ts, cart-service.ts, notification-service.ts, order-service.ts, pizza-service.ts
├── shared/
│   ├── directives/    hot-pizza.ts
│   ├── models/        auth-models.ts, error-response.ts, order-models.ts, pizza-models.ts, …
│   ├── pipes/         ingredient-name-pipe.ts
│   └── ui/            order-card/
└── features/
    ├── admin/         admin-orders/
    ├── auth/          login/
    ├── checkout/
    ├── menu/          home/, pizza-card/, menu-data.ts
    ├── my-pizzas/     my-pizzas/
    ├── orders/        my-orders/
    └── pizza-maker/   pizza-maker/, ingredient-picker/, pizza-price.ts
```

---

<a id="homework"></a>

## 🏠 Homework

Each exercise repeats a page or a pattern from the classes. The API has every endpoint since class 5.

- 🆕 **Register:** a `/register` page that creates a customer with `POST /api/auth/register`. The form checks the [rules](#rules) before anything is sent. On success it says _Welcome, …! You can log in now._ in a snackbar and opens the login page. Logged-out users see _Register_ in the header, and _No account yet? Register_ under the login form. A password the API refuses shows the API's messages.
- 👁️ **Show the password:** on the login and the register page, a button in the password field shows and hides what you typed. Pressing Enter in the password field still logs you in (or creates the account).
- ✏️ **Edit an order:** an _Edit_ button on a Pending order in _My orders_ opens `/orders/:id/edit`, with the order's address and note (`GET /api/orders/{id}`). _Save_ sends `PUT /api/orders/{id}` (the pizzas stay as they were), shows a success message and goes back to _My orders_. Orders that aren't Pending have no _Edit_ button, and one that stopped being Pending in the meantime shows the API's 409 message. Only logged-in users get in. Tip: `:id` arrives as an input.
- 🍕 **Edit a saved pizza:** an _Edit_ button on a card in _My pizzas_ opens `/my-pizzas/:id/edit`, with the pizza's name and description (`GET /api/pizzas/{id}`). _Save_ sends `PUT /api/pizzas/{id}` (the ingredients and the price stay as they were) and goes back to _My pizzas_. Only logged-in users get in.
- 👤 **Profile:** the user's name in the header opens `/profile`, for logged-in users only: the username, the email and the roles (`GET /api/users/me`).
  - The username and the email can be changed (`PUT /api/users/me`), and the header shows the new name at once.
  - The password can be changed with the current one and a new one (`PUT /api/users/me/password`, 204), with a success message. A wrong current password shows the API's message.
- 👥 **Users** (admin only): `/admin/users`, locked like _All orders_, with _Users_ in the header for admins.
  - Every user with their email and role (`GET /api/users`).
  - The role can be changed to Admin or Customer right in the list (`PUT /api/users/{id}/role`).
  - _Delete_ asks for a confirmation first (`DELETE /api/users/{id}`): the user's pizzas and orders are deleted too.
  - The admin can't change or delete their own account on this page.

<a id="rules"></a>
The rules mirror the backend's DTOs:

| Form                   | Field            | Rules                                      | Backend                                                                                 |
| ---------------------- | ---------------- | ------------------------------------------ | --------------------------------------------------------------------------------------- |
| Register, Profile      | username         | required, 3–50 characters                  | `[Required]`, `[StringLength(50, MinimumLength = 3)]`                                   |
| Register, Profile      | email            | required, an email, at most 256 characters | `[Required]`, `[EmailAddress]`, `[StringLength(256)]`                                   |
| Register, new password | password         | required, at least 6 characters            | `[Required]`, plus Identity: a digit, an uppercase letter, a lowercase letter, a symbol |
| Profile                | current password | required                                   | `[Required]`                                                                            |
| Edit order             | address          | required, at most 200 characters           | `[Required]`, `[StringLength(200)]`                                                     |
| Edit order             | note             | at most 500 characters                     | `[StringLength(500)]`                                                                   |
| Edit pizza             | name             | required, at most 100 characters           | `[Required]`, `[StringLength(100)]`                                                     |
| Edit pizza             | description      | at most 500 characters                     | `[StringLength(500)]`                                                                   |

## ✅ By the end of the class

- ❌ _Cancel order_ turns a Pending order into _Cancelled_, without loading the list again
- 🛡️ logged out, `/orders` opens the login page and comes back after the login; as a customer, `/admin/orders` opens the menu; _Log out_ empties the cart and opens the login page
- 👨‍🍳 as the admin, _All orders_ moves an order along: _Pending → Preparing → Delivered → Returned_
- 🧯 a wrong password, a stopped API or a refused change shows a snackbar instead of a red line in the console
- 💾 _Save for later_ in the pizza maker saves the pizza, and _My pizzas_ lists it, deletes it, and puts it in the cart
- 🔍 `npm run lint` passes, and `ng build` shows no warnings, with `my-pizzas` and `admin-orders` among the _Lazy chunk files_

---

## 🔜 After the course

You've built both halves of a real web app: database, API, security and user interface. What we left out on purpose, and what to learn next:

- 🧪 **tests:** unit tests with Vitest (the CLI already set it up), end-to-end tests with Playwright, and xUnit on the backend;
- 🔐 **safer sessions:** refresh tokens and httpOnly cookies;
- 🗃️ **bigger state:** NgRx or SignalStore, when one service per feature isn't enough;
- 📄 **more data:** pagination and search on the API (and `switchMap` on the search box).
