# 📦 Class 09: The login page and the first orders

> 🅰️ Frontend · class 4 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 8: the **Login page** as a typed **Reactive Form**, a header that follows the login, and an **interceptor** that sends the token. Then the cart becomes a real **order**: *Place order* sends it to the API, and *My orders* shows the orders that a **service** keeps in a signal. 🔌 Start your API first, and keep `ana` / `Ana123!` at hand (the first request in `PizzaApp.Api.http`).

## 🗺️ What we do today

1. 🔁 Recap

**🧭 The end of class 8**

2. 📝 The `Login` page: a typed Reactive Form
3. 🧭 The header: what a logged-in user sees
4. 🎟️ `tokenInterceptor`: the token on every request to our API

**📦 Orders**

5. 📦 Order models and `OrderService`
6. 🧾 Checkout: the delivery form and *Place order*
7. 📋 *My orders*: state in a service, and an `OrderCard`

---

## 💡 Used concepts

### 📝 Reactive Forms, typed
```ts
export class Login {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  protected readonly loginForm = this.formBuilder.group({
    userName: ['', Validators.required],
    password: ['', Validators.required],
  });

  protected onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched(); // show every message at once
      return;
    }

    this.authService
      .login(this.loginForm.getRawValue()) // { userName: string; password: string }
      .subscribe(() => this.router.navigate(['/']));
  }
}
```
```html
<form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
  <mat-form-field>
    <mat-label>Username</mat-label>
    <input matInput formControlName="userName" autocomplete="username" />
    @if (loginForm.controls.userName.hasError('required')) {
      <mat-error>The username is required.</mat-error>
    }
  </mat-form-field>
  …
  <button matButton="filled" type="submit">Log in</button>
</form>
```
- 🧩 **The form lives in the class,** and the template only points at it: `[formGroup]` for the form, `formControlName` for each field. `ReactiveFormsModule` goes into the component's `imports`.
- 🔷 **Typed:** `getRawValue()` has exactly the `LoginRequestDto` shape, so it goes straight to the service. `.value` doesn't: a disabled field would be missing from it, so its type is `Partial<…>` and doesn't compile there.
- 🚫 **`NonNullable`:** a reset puts `''` back into the fields, not `null`.
- 🔴 **`<mat-error>` inside the `<mat-form-field>`:** Material shows it once the field was touched, or the form was submitted.
- ⏎ **`(ngSubmit)` with a `type="submit"` button:** Enter in any field submits the form. A button in a form that isn't the submit button needs `type="button"`, or it submits the form too.

The rules mirror the backend's DTOs, so most mistakes are caught before the request:

| Form | Field | `Validators` | Backend DTO |
|---|---|---|---|
| Login | `userName`, `password` | `required` | `[Required]` |
| Checkout | `addressTo` | `required`, `maxLength(200)` | `[Required]`, `[StringLength(200)]` |
| Checkout | `description` | `maxLength(500)` | `[StringLength(500)]` |

⚖️ **The server has the final word:** it checks the same rules again and answers 400 when they don't hold. For now a failed request is only a red `ERROR HttpErrorResponse` in the console; the next class shows the API's message to the user.

### 🆕 Signal Forms: Angular's newer forms
The same login form, with the API that's stable since Angular 22. Newer projects and the docs use it, while most existing code uses Reactive Forms, so we do too. Just learn to recognize it:
```ts
import { form, FormField, FormRoot, required } from '@angular/forms/signals';

private readonly credentials = signal({ userName: '', password: '' }); // the model is a signal

protected readonly loginForm = form(this.credentials, (path) => {
  required(path.userName, { message: 'The username is required.' });
  required(path.password, { message: 'The password is required.' });
});
```
```html
<input matInput [formField]="loginForm.userName" />
@for (error of loginForm.userName().errors(); track error.kind) {
  <mat-error>{{ error.message }}</mat-error>
}
```
- 🔁 **The form writes straight into the model signal,** and the rules are functions with their messages. `[formRoot]` on the `<form>` and a `submission` option handle the submit.

### 🧭 The header follows the login
```html
@if (authService.isLoggedIn()) {
  <a matButton routerLink="/pizza-maker" routerLinkActive="active">Make a pizza</a>
  <a matButton routerLink="/orders" routerLinkActive="active">My orders</a>
}
…
@if (authService.isLoggedIn()) {
  <!-- {{ authService.userName() }} and Log out -->
} @else {
  <a matButton routerLink="/login" routerLinkActive="active">Log in</a>
}
```
- 🚦 **The header reads signals,** so logging in or out changes it at once, with no reload.
- 🔑 ***Log in* opens the Login page,** which replaces class 8's temporary *Log in as admin* button.
- 🔓 **A hidden link isn't security:** logged out, typing `/pizza-maker` still opens the page. Route guards (next class) and the API's own checks are what lock it.

### 🎟️ An interceptor: the token on every request
```ts
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).token();

  // Only our own API gets the token: it must never be sent to another site
  if (!token || !req.url.startsWith(environment.apiUrl)) {
    return next(req);
  }

  // Requests can't be changed, so we send a copy with the extra header
  return next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};
```
```ts
// app.config.ts
provideHttpClient(withInterceptors([tokenInterceptor])),
```
- 🔗 **Like middleware in ASP.NET Core:** every request of the `HttpClient` passes through, and `next(req)` sends it on. No service has to remember the token.
- 🧊 **Requests are immutable:** `clone()` makes a copy with the header.
- 📜 **Older code:** a class that implements `HttpInterceptor`, registered with `HTTP_INTERCEPTORS` and `withInterceptorsFromDi()`.

---

### 📦 State in a service
`OrderService` keeps the orders, and every page that shows orders reads them:
```ts
@Service()
export class OrderService {
  private readonly ordersState = signal<OrderDto[]>([]); // only the service can change them
  readonly orders = this.ordersState.asReadonly();

  private readonly loadingState = signal(false);
  readonly loading = this.loadingState.asReadonly();

  load(): void {
    this.ordersState.set([]); // never show the previous user's orders while loading
    this.loadingState.set(true);
    this.http
      .get<OrderDto[]>(this.url)
      .pipe(finalize(() => this.loadingState.set(false))) // after success AND after an error
      .subscribe((orders) => this.ordersState.set(orders));
  }

  create(order: AddOrderDto): Observable<OrderDto> {
    return this.http.post<OrderDto>(this.url, order); // the page decides what happens next
  }
}
```
- 🔒 **The same pattern as the cart:** a private writable signal, `asReadonly()` for everyone else.
- 📥 **`load()` subscribes itself** and puts the answer into the signal; the page only calls it in `ngOnInit` and reads `orders()` and `loading()`.
- ↩️ **`create()` returns the Observable,** like `login()`: only the Checkout page knows what to do after a successful order (empty the cart, open *My orders*).
- 🧹 **`finalize`** runs once at the end, however the request ended: C#'s `finally`. That's why a spinner or a disabled button always comes back.
- 💶 **No price in `AddOrderDto`:** the server adds up the pizzas, and *My orders* shows the server's `totalPrice`.

### 🧾 A busy button
```ts
protected readonly placing = signal(false);

this.placing.set(true);
this.orderService
  .create(order)
  .pipe(finalize(() => this.placing.set(false)))
  .subscribe((createdOrder) => { /* empty the cart, snackbar, /orders */ });
```
```html
<button matButton="filled" type="submit" [disabled]="placing()">Place order</button>
```
- 🖱️🖱️ **A double-click would place two orders:** the button stays disabled until the API answers.
- 🧺 **The cart is emptied only in the `subscribe` callback,** after the API accepted the order. If the order fails, the cart is still there.

### 🔔 A snackbar for messages (handout)
```ts
this.notificationService.success(`Order #${createdOrder.id} placed. Total: € ${createdOrder.totalPrice.toFixed(2)}`);
```
- 📣 **`NotificationService`** is the only code that knows Material's `MatSnackBar`: a page says *what* to tell the user, not *how*. Its `error()` is ready for the next class.

### 🃏 A page and a card
```html
<!-- my-orders.html: the page talks to the service -->
@if (orderService.loading()) {
  <mat-spinner />
} @else {
  <div class="orders">
    @for (order of orderService.orders(); track order.id) {
      <app-order-card [order]="order" />
    } @empty {
      <p>No orders yet. <a routerLink="/pizza-maker">Make your first pizza</a>.</p>
    }
  </div>
}
```
```ts
// order-card.ts: the card shows what it gets, and knows nothing about the API
export class OrderCard {
  readonly order = input.required<OrderDto>();
}
```
- 🧱 **A smart page and a simple card,** like `Home` and `PizzaCard` in class 7. The card sits in `shared/ui/` because more than one page will show orders.
- ⏳ **`loading()`** shows a spinner until the answer arrives, and `@empty` covers a user with no orders yet.
- 📅 **`date` pipe:** `{{ order().createdDate | date: 'd MMM y, HH:mm' }}` turns the API's UTC text into *28 Oct 2026, 19:30*, in the browser's time zone. `DatePipe` goes into `imports`, like `CurrencyPipe`.

### 🗂️ Where things go
```
src/app/
├── core/
│   ├── interceptors/  token-interceptor.ts
│   ├── layout/        header/, not-found/
│   └── services/      auth-service.ts, cart-service.ts, notification-service.ts, order-service.ts
├── shared/
│   ├── models/        auth-models.ts, order-models.ts, pizza-models.ts, …
│   └── ui/            order-card/
└── features/
    ├── auth/          login/
    ├── checkout/      checkout.ts · .html · .scss
    └── orders/        my-orders/
```

---

## ✅ By the end of the class
- 🔑 *Log in* opens the menu with your name and *Log out* in the header; empty fields show their messages without sending anything
- 🧭 logged out, the header shows *Menu*, the cart and *Log in*; logged in, also *Make a pizza* and *My orders*
- 🎟️ the requests to our API carry `Authorization: Bearer …` (DevTools → Network)
- 🧾 *Place order* sends the cart to the API, shows the order's number and total in a snackbar, empties the cart, and opens *My orders*, where the order is *Pending* with the server's total
- 🔍 `ng build` shows no warnings, with `login` and `my-orders` among the *Lazy chunk files*

---

🔜 **Next class:** first what we didn't get to today: *Cancel* on a Pending order (`<ng-content>` and `changeStatus()`), **route guards** for logged-in users and for the admin, the admin's **orders board**, and an **error interceptor** that shows every API error to the user. Then saved pizzas and the wrap-up.
