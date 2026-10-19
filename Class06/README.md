# 🅰️ Class 06: Angular foundations

> 🅰️ Frontend · class 1 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

The API is finished, so now we build the app that uses it: a new **Angular 22** app, **TypeScript** for C# developers, **components**, **signals** and **routing**. By the end of the class, the **menu page** shows ten pizzas.

## 🗺️ What we do today

1. 🖥️ A single-page app: the finished app, and how it uses our API

**🧰 The tools and the language**

2. 🛠️ A new Angular app: `ng new`, `npm start`, and a tour of the generated files
3. 📄 Handout: Angular Material, ESLint and Prettier
4. 🔷 TypeScript for C# developers

**🧩 Components**

5. 🧩 Components and templates: the `Header` inside `App`, interpolation, binding, `@if` and `@for`
6. 🚦 Signals: `signal`, `computed`, `set` and `update`, and why Angular needs them
7. 🍕 The menu: the `Home` page with a card for every pizza, in a CSS grid
8. 🖼️ Images with `NgOptimizedImage`

**🧭 Pages**

9. 🧭 Routing: the first route, lazy loading, a page title and `<router-outlet />`

🏠 No homework today.

---

## 💡 Used concepts

### 🖥️ A single-page app
The browser downloads `index.html` and the app's JavaScript **once**. After that, Angular builds every page **in the browser**: a click swaps the view, without reloading the page. From class 8 on, the data comes from our API as JSON, with the token in a header (the sketch is in [class 5](../Class05/README.md)).

### 🛠️ Node, npm and the Angular CLI

| | What it is | In .NET |
|---|---|---|
| **Node.js** | runs JavaScript outside the browser: the CLI, the build, the dev server | the .NET SDK |
| **npm** | downloads packages from npmjs.com | NuGet |
| `package.json` | the packages with their versions, and the scripts (`npm start`, `npm run lint`) | the `.csproj` files + `Directory.Packages.props` |
| `node_modules/` | the downloaded packages. Never committed: `npm install` brings them back | the NuGet cache |
| **Angular CLI** (`ng`) | creates the app, generates files, serves and builds it | `dotnet new`, `dotnet run`, `dotnet build` |

```powershell
ng new pizza-app --style=scss --ssr=false --skip-tests   # "Which AI tools…?" → None
cd pizza-app
npm start                                                # ng serve → http://localhost:4200
```
🔁 `ng serve` rebuilds on every save, and the browser reloads by itself. **Ctrl + C** stops it.

### 📁 What ng new creates

| File | What it does |
|---|---|
| `src/index.html` | the only HTML page. Angular starts at `<app-root>` |
| `src/main.ts` | starts the app: `bootstrapApplication(App, appConfig)` |
| `src/app/app.ts` · `.html` · `.scss` | `App`, the root component: its class, template and styles |
| `src/app/app.config.ts` | the providers for the whole app: Angular's `Program.cs` |
| `src/app/app.routes.ts` | the pages and their URLs (empty for now) |
| `src/styles.scss` | the global styles |
| `public/` | files served as they are: `public/images/bacon.png` → `/images/bacon.png` |
| `angular.json` | the CLI's settings: how to build and serve, and the defaults for `ng generate` |
| `tsconfig.json` | TypeScript's settings |

What Angular 22 does by default:
- 🧩 **standalone components:** each component lists what it uses itself, with no `NgModule`s
- ⚡ **zoneless:** Angular updates the screen when it knows that something changed (see [signals](#signals))
- 📛 **short file names:** `header.ts` holds the class `Header`. Older projects have `header.component.ts` with `HeaderComponent`
- 🔒 **strict:** like `Nullable` in C#, `null` is not a `string`, and the templates are type-checked too

### ➕ ng add: Angular Material, ESLint and Prettier
`ng add` installs a package **and** sets it up in the project:
```powershell
ng add @angular/material   # the colors: Azure/Blue
ng add angular-eslint
```
- 🎨 **Angular Material:** ready-made components: toolbar, buttons, cards, chips, icons. `ng add` puts the theme into `styles.scss` and the fonts into `index.html`. A component imports only what it uses: `MatToolbarModule`, `MatCardModule`, …
- 🔍 **ESLint:** finds problems in the code and in the templates, accessibility included (an `<img>` without `alt`). `npm run lint` checks the whole app.
- ✨ **Prettier:** formats the code by the rules in `.prettierrc` (single quotes, 100 characters per line). In VS Code: Settings → *Format On Save* ✔ and *Default Formatter* → Prettier.

### 🔷 TypeScript for C# developers
TypeScript is JavaScript **with types**. The types are checked during the build and then removed: the browser runs plain JavaScript.

| | C# | TypeScript |
|---|---|---|
| a variable | `var count = 0;` | `let count = 0;` · `const shop = 'Pizza App';` (can't be reassigned) |
| a type | `decimal price` | `price: number` (one number type, written after the name) |
| null | `string? description` | `description: string \| null`, or optional: `description?: string` |
| the shape of some data | a DTO class | `interface MenuPizza { name: string; price: number }` |
| a fixed set of values | `enum PizzaSize` | `type PizzaSize = 'Small' \| 'Medium' \| 'Large';` (a **union type**) |
| generics | `List<string>` | `string[]` or `Array<string>` · `signal<number>(0)` |
| a lambda | `pizza => pizza.Price` | `(pizza) => pizza.price` |
| string interpolation | `$"Total: {total}"` | `` `Total: ${total}` `` |
| equality | `==` | `===`: always three, because `==` converts types first |
| `?.` and `??` | `pizza?.Name ?? "none"` | `pizza?.name ?? 'none'`: the same |
| another file | `using PizzaApp.Dtos;` | `import { MenuPizza } from './menu-data';`, and `export` in that file |

🧩 **`as const`: one list, and a type made from it.** Our ingredients are the API's enum values, as text:
```ts
export const INGREDIENTS = ['TomatoSauce', 'Mozzarella', 'Ham', …] as const;

// 'TomatoSauce' | 'Mozzarella' | 'Ham' | …
export type Ingredient = (typeof INGREDIENTS)[number];

const topping: Ingredient = 'Pineapple';   // ❌ doesn't compile
```
`as const` makes the array read-only and keeps every string as its own type, so the list and the type can never disagree.

---

### 🧩 Components
A component is a **class** (the data and the logic), a **template** (the HTML) and **styles**. `ng g c core/layout/header` creates `header.ts`, `header.html` and `header.scss`:
```ts
@Component({
  selector: 'app-header',                       // in a template: <app-header />
  imports: [MatToolbarModule, MatIconModule],   // everything this template uses
  templateUrl: './header.html',
  styleUrl: './header.scss',                    // styles for this component only
})
export class Header {}
```
```html
<!-- app.html -->
<app-header />

<main class="page">
  …
</main>
```
- 📥 **`imports` is per component,** a `using` for its template. A missing import stops the build (NG8001 *'mat-toolbar' is not a known element*), and an unused one gets a warning (NG8113).
- 🎨 **Component styles stay in their component:** `.brand` in `header.scss` can't change anything outside the header.
- 🌳 **The app is a tree of components:** `App` → `Header` and the current page (`Home`).

### 📝 Templates

| Syntax | What it does | Example |
|---|---|---|
| `{{ … }}` | **interpolation:** shows a value as text | `<h1>Welcome to {{ shopName }}</h1>` |
| `[property]="…"` | **property binding:** sets an element's property from a value | `<button [disabled]="pizzaCount >= 5">` |
| `(event)="…"` | **event binding:** runs a method when the event happens | `<button (click)="addPizza()">` |
| `@if` · `@else` | shows a block or not | below |
| `@for` + `track` · `@empty` | repeats a block for every item; `@empty` when there are none | below |

```html
@if (pizzaCount === 0) {
  <p>Nothing ordered yet.</p>
} @else if (pizzaCount >= 5) {
  <p>That's the maximum.</p>
}

<ul>
  @for (topping of toppings; track topping) {
    <li>{{ topping }}</li>
  } @empty {
    <li>No toppings</li>
  }
</ul>
```
- 🔑 **`track` is required:** it tells Angular which item is which (an id, or the value itself in a list of strings), so after a change Angular updates only the items that changed.
- 🆚 **Like Razor, but live:** Razor builds the HTML once, on the server. An Angular template stays connected to its component and follows its data.
- 🔒 **The build checks templates too:** a typo like `{{ shopNam }}` doesn't compile.

<a id="signals"></a>
### 🚦 Signals
A **signal** is a value that tells Angular when it changes:
```ts
export class Home {
  protected readonly pizzaCount = signal(0);                            // a signal<number>
  protected readonly total = computed(() => this.pizzaCount() * 8.5);   // follows pizzaCount

  protected addPizza(): void {
    this.pizzaCount.update((count) => count + 1);   // a new value from the old one
  }

  protected reset(): void {
    this.pizzaCount.set(0);                         // a new value
  }
}
```
```html
<p>{{ pizzaCount() }} pizzas in your order: {{ total() }} €</p>
```
- 📖 **Read a signal by calling it:** `pizzaCount()`, in the class and in the template.
- ✏️ **Change it with `set` or `update`,** never with `=`.
- 🧮 **`computed` is a read-only signal** made from other signals. It's calculated again only when one of them changes, and it has no `set`.
- ⚡ **Why Angular needs them:** Angular 22 is **zoneless**, so nothing watches every timer or every answer from a server. Angular draws a component again only when it knows that something changed: an event in its template, a new input, or **a signal that its template reads**. A timer that changes a plain field leaves the screen behind; with a signal, the screen follows:
  ```ts
  constructor() {
    // with a plain field and this.seconds++, the screen would stay at 0
    setInterval(() => this.seconds.update((value) => value + 1), 1000);
  }
  ```
- 📏 **Our rule: state that changes is a signal.**

### 🍕 The menu
`Home` shows a Material card for every pizza, with a chip for every ingredient:
```ts
export class Home {
  protected readonly menu = MENU;   // from menu-data.ts
}
```
```html
<div class="menu">
  @for (pizza of menu; track pizza.id) {
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ pizza.name }}</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>{{ pizza.description }}</p>
        <mat-chip-set aria-label="Ingredients">
          @for (ingredient of pizza.ingredients; track ingredient) {
            <mat-chip>{{ ingredient }}</mat-chip>
          }
        </mat-chip-set>
      </mat-card-content>
    </mat-card>
  }
</div>
```
- 🍽️ **The menu is frontend data** (`menu-data.ts`, a handout): the API has no menu endpoint. `MenuPizza` is an interface, and its `ingredients` are `Ingredient[]`, the type made with `as const`.
- 🔁 **`@for` inside `@for`:** the pizzas, and inside each card its ingredients.
- 📥 **Every Material piece is in `Home`'s `imports`:** `MatCard`, `MatCardHeader`, `MatCardTitle`, `MatCardContent`, `MatChipSet`, `MatChip`.
- 🔲 **A CSS grid** places the cards: as many columns as fit, with no JavaScript:
  ```scss
  .menu {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(16rem, 1fr));
    gap: 16px;
  }
  ```

### 🗂️ core, shared and features
```
src/app/
├── app.ts · app.html · app.scss    the shell: the header + the current page
├── app.config.ts · app.routes.ts   the providers and the pages
├── core/layout/                    used once in the whole app: header/
├── shared/models/                  used everywhere: pizza-models.ts
└── features/menu/                  one folder per area of the app: menu-data.ts, home/
```

### 🖼️ NgOptimizedImage
`ngSrc` instead of `src` hands the image to Angular's image directive:
```html
<img
  mat-card-image
  [ngSrc]="pizza.image"
  [width]="pizza.imageWidth"
  [height]="pizza.imageHeight"
  [alt]="pizza.name"
/>
```
- 📐 **`width` and `height` are required:** the photo's real size, so the browser saves the space before the image arrives and the page doesn't jump. CSS then scales it (`width: 100%; height: auto`).
- 💤 **Lazy by default:** an image further down the page loads only when you scroll to it (`loading="lazy"`).
- 🚀 **`priority` for the first big image,** our logo: it loads straight away, with `fetchpriority="high"`.
- 🧑‍🏫 **It checks your images** in the browser console, while you develop: the biggest image without `priority` (NG02955), a `width` and `height` that don't match the photo (NG02952), no size at all (NG02954, an error).

### 🧭 Routing
```ts
export const routes: Routes = [
  {
    path: '',
    title: 'Menu · Pizza App',
    loadComponent: () => import('./features/menu/home/home').then((m) => m.Home),
  },
];
```
```html
<!-- app.html: the router puts the current page into <router-outlet /> -->
<app-header />

<main class="page">
  <router-outlet />
</main>
```
- ⚙️ **`provideRouter(routes)`** in `app.config.ts` (already there from `ng new`) gives the routes to the router, and `App` imports `RouterOutlet`.
- 💤 **Lazy loading:** `loadComponent` with `import(…)` puts each page in its own JavaScript file, downloaded the first time someone opens the page. `ng build` lists them as *Lazy chunk files*.
- 🏷️ **`title`** is the text on the browser tab.

---

## 🏠 Homework
None today. Make sure `npm start` shows the menu on your computer: class 7 builds on it.

## ✅ By the end of the class
- 🍕 http://localhost:4200 shows the logo and 10 pizza cards, each with its photo, description and ingredients, in as many columns as fit
- 🧭 the header is in `App`, above `<router-outlet />`, and the menu page has its own tab title
- 🔍 `npm run lint` passes, and `ng build` shows no warnings, with `home` among the *Lazy chunk files*

---

🔜 **Next class:** first what we didn't get to today: a `PizzaCard` with `input()`, the header links with `routerLink` and `routerLinkActive`, and a 404 page. Then the pizza maker and the cart: `output()` and `model()`, the component lifecycle, a service with `inject()`, `effect()`, a custom pipe and a custom directive. The API comes back in class 8.
