# 🛒 Class 08: The cart and the first login

> 🅰️ Frontend · class 3 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 7: a **cart** that the whole app shares through a **service**, the **cart page**, and two display helpers, a **pipe** and a **directive**. Then the app talks to our **API** for the first time: **Observables**, and an **`AuthService`** that logs in and remembers who is logged in, even after a reload. 🔌 The app calls the API at `https://localhost:5004`: start the API with the **`https`** profile.

## 🗺️ What we do today

1. 🔁 Recap

**🧭 Services, pipes, directives**

2. 🛒 `CartService`: a service with `@Service()` and `inject()`, and the cart badge
3. 💾 `effect()`: the cart survives a page reload
4. 🧾 The `Checkout` page: the cart, remove, the total
5. 🔤 Pipes: the built-in `currency` and `uppercase`, and our own `ingredientName`
6. 🌶️ A custom directive: `HotPizza`

**🌐 Talking to the API**

7. ⚙️ Environments: the API's address in one place, and models that mirror the DTOs
8. 📡 `HttpClient` and Observables: the first request
9. 🔐 `AuthService`: the login in a signal and in `localStorage`

---

## 💡 Used concepts

### 🛒 Services and inject()
The pizza maker adds pizzas, the header counts them, and the cart page lists them: one cart, shared by all three. That's a **service**:
```powershell
ng g s core/services/cart-service   # cart-service.ts → CartService
```
```ts
@Service() // one instance for the whole app
export class CartService {
  private readonly itemsState = signal<CartItem[]>(readSavedCart()); // only the service can change it

  readonly items = this.itemsState.asReadonly(); // everyone else can only read it
  readonly count = computed(() => this.itemsState().length);

  add(pizza: Omit<CartItem, 'id'>): void {
    this.itemsState.update((items) => [...items, { ...pizza, id: crypto.randomUUID() }]);
  }

  remove(id: string): void {
    this.itemsState.update((items) => items.filter((item) => item.id !== id));
  }

  clear(): void {
    this.itemsState.set([]);
  }
}
```
```ts
export class Header {
  protected readonly cartService = inject(CartService); // the same instance as in the pizza maker
}
```
```html
<a matIconButton routerLink="/checkout" aria-label="Cart" [matBadge]="cartService.count()">
  <mat-icon>shopping_cart</mat-icon>
</a>
```
- 💉 **`inject()`** works for our own services exactly as for the `Router` in class 7.
- 1️⃣ **`@Service()`:** Angular creates the service the first time someone injects it, and everyone gets that same instance, like `AddSingleton`. Older code writes `@Injectable({ providedIn: 'root' })`, with the same result.
- 🔒 **The state stays inside:** a private writable signal, and `asReadonly()` for everyone else, like a private `List<T>` behind an `IReadOnlyList<T>`. `cartService.items.set([])` doesn't compile.
- 🆕 **A new array, never `push`:** a signal tells Angular about a change only when it gets a **new** value. `items.push(…)` changes the same array, so the badge and the cart miss the pizza. `[...items, pizza]` makes a new one, and so do `filter` in `remove()` and `set([])` in `clear()`.
- ✂️ **`Omit<CartItem, 'id'>`** is `CartItem` without `id`: the caller sends a pizza, the service adds the id.

### 💾 effect(): sync with the world outside Angular
```ts
constructor() {
  // runs once now, and again every time itemsState changes
  effect(() => {
    const jsonString = JSON.stringify(this.itemsState());
    localStorage.setItem(CART_KEY, jsonString);
  });
}
```
- 🔄 **An effect runs code when a signal it reads changes.** It's for copying a signal to something Angular doesn't manage: `localStorage`, the page title, a chart library.
- 🚫 **Not for a value made from other signals:** that's `computed`, like the cart's `total`.
- 🗄️ **`localStorage`** keeps text per site, even after the browser closes: `JSON.stringify` on the way in, `JSON.parse` on the way out. See it in DevTools → *Application* → *Local Storage* → `pizza-app.cart`.
- 📥 **On startup,** `readSavedCart()` gives the signal its first value: the saved cart, or an empty one if there's nothing saved or the text isn't valid JSON (the `catch`).

### 🔤 Pipes
A pipe formats a value in the template, and leaves the data as it is:
```html
{{ price() | currency: 'EUR' }}           <!-- 3.6 → €3.60 -->
{{ item.name | uppercase }}               <!-- Margherita → MARGHERITA -->
{{ ingredient | ingredientName }}         <!-- ChilliPepper → Chilli pepper -->
{{ item.ingredients | ingredientName }}   <!-- a list → Tomato sauce, Mozzarella -->
```
- 🧰 **Built in:** `currency`, `uppercase`, `date`, `json`, … A pipe goes into the component's `imports` like everything else: `CurrencyPipe`, `UpperCasePipe`.
- 🛠️ **Our own:** `ng g p shared/pipes/ingredient-name` creates `IngredientNamePipe`:
  ```ts
  @Pipe({ name: 'ingredientName' }) // the name used in templates
  export class IngredientNamePipe implements PipeTransform {
    transform(value: Ingredient | readonly Ingredient[]): string { … }
  }
  ```
- 👁️ **Display only:** the cart still holds `TomatoSauce`; only the screen says *Tomato sauce*.
- ⚡ **Pure:** Angular calls `transform` again only when the value changes. A method in a template (`{{ format(x) }}`) runs every time the component is drawn.
- ⚠️ **Not RxJS's `pipe()`** further down: same word, two unrelated things. A `|` in the template is Angular; `.pipe(` in TypeScript is RxJS.

### 🌶️ A custom directive
A component brings its own template. A **directive** adds behavior to an element that's already there:
```ts
@Directive({
  selector: '[appHotPizza]', // an attribute, so it fits on any element
  host: { '[class.hot]': 'isHot()' }, // a binding on the element the directive sits on
})
export class HotPizza {
  readonly ingredients = input<readonly Ingredient[]>([], { alias: 'appHotPizza' });

  protected readonly isHot = computed(() => this.ingredients().includes('ChilliPepper'));
}
```
```html
<mat-card class="card" [appHotPizza]="pizza().ingredients">   <!-- the menu: a HOT stamp -->
<section class="summary" [appHotPizza]="ingredients()">        <!-- the pizza maker: a red outline -->
```
- 🏷️ **`host`:** the directive sets `[class.hot]` on its element, as if the template had written it there.
- 🔤 **`alias`:** the directive's own name carries the value, so one attribute does both.
- 🧩 **We've used directives all along:** `routerLink`, `routerLinkActive`, `ngSrc`, `matButton`.
- 📜 **Older code:** `@HostBinding('class.hot')`, or `ElementRef` with `Renderer2`.

---

### ⚙️ Environments: settings per build
```powershell
ng g environments
```
```ts
// src/environments/environment.development.ts: used by npm start (ng serve)
export const environment = {
  apiUrl: 'https://localhost:5004/api',
};
```
- 🔀 **Two files, one import:** the code always imports `environments/environment`. `ng build` uses `environment.ts`, and for `ng serve` the `fileReplacements` in `angular.json` swap in `environment.development.ts`. Like `appsettings.json` and `appsettings.Development.json`.
- 🔌 **The address must match the API's profile:** `https://localhost:5004` is the `https` profile in the API's `launchSettings.json`. If you run the `http` profile, write `http://localhost:5004/api` in both files.
- 🙈 **No secrets in here:** the browser downloads the whole app, so anyone can read these values. The API's secrets stay in the API's user-secrets.

### 🔷 Models that mirror the DTOs
```ts
// shared/models/auth-models.ts: LoginResponseDto from the backend, field by field
export interface LoginResponseDto {
  token: string;
  expiresAt: string; // JSON has no date type: a date arrives as text (UTC)
  userName: string;
  roles: Role[];      // Role = 'Admin' | 'Customer'
}
```
- 📐 **An interface, not a class:** it only describes the JSON's shape, so nothing is left of it when the app runs. Its fields are camelCase, like the JSON.
- 🔍 **The names match the backend** (`LoginRequestDto`, `UserDto`, …), so each C# class and its TypeScript interface are easy to find. Data that exists only in the browser has its own types: `MenuPizza`, `CartItem`, `PizzaSize`.

### 📡 HttpClient and Observables
Services call the API, and pages call the services:
```ts
// auth-service.ts
private readonly http = inject(HttpClient);
private readonly controller = 'auth';
private readonly url = `${environment.apiUrl}/${this.controller}`;

login(credentials: LoginRequestDto): Observable<LoginResponseDto> {
  return this.http.post<LoginResponseDto>(`${this.url}/login`, credentials);
}
```
```ts
// header.ts: whoever needs the answer subscribes
protected loginAsAdmin(): void {
  this.authService
    .login({ userName: 'admin', password: 'Admin123!' })
    .subscribe((loggedInUser) => console.log(loggedInUser));
}
```
An **Observable** is a value that arrives later, like a `Task<T>` in C#, but lazier:

| | Promise (`async` / `await`) | Observable (RxJS) | Signal |
|---|---|---|---|
| gives | one value, later | any number of values, over time | always a current value |
| starts | right away | when someone subscribes | |
| read with | `await`, `.then()` | `.subscribe()` | `count()` |
| we use it for | | HTTP requests | state that the screen shows |

- 💤 **Nothing is sent until you subscribe:** without `.subscribe()`, there's no request in DevTools → Network. And every `subscribe` sends the request **again**.
- 1️⃣ **An HTTP Observable gives one value and completes,** so it never needs an unsubscribe.
- 🏷️ **`post<LoginResponseDto>`** only tells TypeScript what comes back. Nothing checks the JSON when the app runs, so the interface must match the DTO.
- 🎁 **`HttpClient` is ready in every Angular 22 app,** with no setup, and it sends its requests with `fetch`: DevTools → Network → *Fetch/XHR*.
- ❌ **A failed request** (a 401, the API not running) ends the Observable with an error. For now that's only a red `ERROR HttpErrorResponse` in the console; class 9 shows the API's message to the user.
- 🧪 ***Log in as admin* is temporary:** it lets us log in before the login page exists. The page with a real form replaces it in class 9.

### 🔧 pipe(), map and tap
`pipe()` puts **operators** between the answer and `subscribe`. Each one works on the value on its way, like a LINQ chain:
```ts
this.authService
  .login({ userName: 'admin', password: 'Admin123!' }) // Observable<LoginResponseDto>
  .pipe(map((user) => user.userName)) // a new Observable<string>
  .subscribe((userName) => console.log(userName)); // admin
```
Step by step:
1. `login()` returns an Observable: the request is described, not sent.
2. `.pipe(…)` returns a **new** Observable with the operators in it. Still nothing is sent.
3. `.subscribe(…)` sends the request.
4. The answer goes through the operators, top to bottom, and what comes out of the last one reaches the `subscribe` callback.
5. On an error (a 401), the operators are skipped, and the error goes straight to the end.

- 🔁 **`map` changes the value,** like LINQ's `Select`: here the whole answer becomes just the name.
- 👂 **`tap` does something with the value and passes the same value on,** unchanged. `AuthService` uses it to save the session (below).
- 📥 **Both come from `rxjs`:** `import { map, tap } from 'rxjs';`.

---

### 🔐 AuthService: the session in a signal
```ts
const SESSION_KEY = 'pizza-app.session';

@Service()
export class AuthService {
  private readonly userSession = signal<LoginResponseDto | null>(readSavedUserSession());

  readonly isLoggedIn = computed(() => this.userSession() !== null);
  readonly isAdmin = computed(() => this.userSession()?.roles.includes('Admin') ?? false);
  readonly userName = computed(() => this.userSession()?.userName ?? null);

  login(credentials: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http
      .post<LoginResponseDto>(`${this.url}/login`, credentials)
      .pipe(tap((user) => this.saveUserSession(user))); // the signal + localStorage
  }

  logout(): void {
    localStorage.removeItem(SESSION_KEY);
    this.userSession.set(null);
  }
}
```
```html
<!-- header.html -->
@if (authService.isLoggedIn()) {
  <span class="user">
    <mat-icon>person</mat-icon>
    {{ authService.userName() }}
  </span>
  <button matButton type="button" (click)="logout()">Log out</button>
} @else {
  <button matButton type="button" (click)="loginAsAdmin()">Log in as admin</button>
}
```
What happens on *Log in as admin*, step by step:
1. `subscribe()` in the header sends the POST.
2. **200:** `tap` calls `saveUserSession(user)`, which puts the session into the `userSession` signal and into `localStorage`.
3. `tap` passes the same value on to the `subscribe` callback, which logs it.
4. The header reads `isLoggedIn()` and `userName()`, so it shows *admin* and *Log out* at once.
5. **401** (a wrong password): `tap` is skipped, so no session is saved.

- 🚦 **One signal, the rest is `computed`:** log in or out, and everything that reads `isLoggedIn()` or `userName()` follows by itself, with no reload.
- ↩️ **`login()` returns the Observable:** the service keeps the session, and the caller decides what happens after a successful login. In class 9 the login page will open the menu.
- 💾 **`localStorage` keeps the session after a reload.** On startup, `readSavedUserSession()` throws away a session whose `expiresAt` has passed, so an expired token never counts as logged in.
- 🔓 **The token is no secret from the user:** jwt.io shows what's inside (class 2). Any script on the page can read `localStorage` too; the safer place, an httpOnly cookie, comes up in the wrap-up.

### 🗂️ Where things go
```
src/
├── environments/      environment.ts, environment.development.ts
└── app/
    ├── core/
    │   ├── layout/        header/, not-found/
    │   └── services/      auth-service.ts, cart-service.ts
    ├── shared/
    │   ├── directives/    hot-pizza.ts
    │   ├── models/        auth-models.ts, user-models.ts, pizza-models.ts, pizza-size.ts
    │   └── pipes/         ingredient-name-pipe.ts
    └── features/
        ├── menu/          menu-data.ts, home/, pizza-card/
        ├── pizza-maker/   pizza-price.ts, pizza-maker/, ingredient-picker/
        ├── checkout/      checkout.ts · .html · .scss
        └── auth/          login/ (only the start: the form comes in class 9)
```

---

## ✅ By the end of the class
- 🛒 *Add to cart* puts the pizza in the cart: the badge counts the pizzas, and the cart survives a page reload
- ➕ *Add and make another* puts the pizza in the cart and starts a new one, without leaving the page
- 🗑️ the cart page lists the pizzas, a pizza can be removed, and the total follows; *Empty cart* removes them all, also after a reload
- 🔤 the ingredients read *Chilli pepper*, not `ChilliPepper`, and the prices *€3.60*
- 🌶️ Mexicana has a *HOT* stamp on the menu, and chilli pepper puts a red outline around the pizza maker's summary
- 🔑 *Log in as admin* shows *admin* and *Log out* in the header, also after a reload, and *Log out* brings the button back
- 📡 DevTools → Network shows the `POST` to `/api/auth/login`, and *Local Storage* shows `pizza-app.cart` and `pizza-app.session`
- 🔍 `ng build` shows no warnings, with `checkout` among the *Lazy chunk files*

---

🔜 **Next class:** first what we didn't get to today: a `tokenInterceptor` that sends the token with every request to our API, the **Login** page as a typed Reactive Form (it replaces *Log in as admin*), the **Register** page, and a header that shows each user only their own links. Then orders, guards and errors.
