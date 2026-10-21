# 🍕 Class 07: The pizza maker

> 🅰️ Frontend · class 2 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](../Class01/Initial-Setup.md)

First we finish class 6: a **`PizzaCard`** that gets its pizza through an **input**, the **header links** and a **404 page**. Then the menu leads somewhere: *Make it* on a card opens a **pizza maker** to build your own pizza, with a price that follows what you pick. The API is still off today.

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

🏠 No homework today.

---

## 💡 Used concepts

### 🍕 input(): data from the parent
`Home` has the menu, and it gives each pizza to a `PizzaCard`:
```ts
export class PizzaCard {
  readonly pizza = input.required<MenuPizza>(); // a signal that the parent fills
}
```
```html
<!-- home.html: [pizza] is the card's input, pizza is the loop variable -->
@for (pizza of menu; track $index) {
  <app-pizza-card [pizza]="pizza" />
}

<!-- pizza-card.html: an input is a signal, so it's read with () -->
<mat-card-title>{{ pizza().name }}</mat-card-title>
```
- ❗ **`required`:** an `<app-pizza-card />` without `[pizza]` doesn't compile (NG8008).
- 🧱 **A smart page, a simple card:** `Home` knows where the data comes from, and `PizzaCard` only shows what it gets, so it can be reused anywhere.
- 📜 **Older code** (and many tutorials) uses the `@Input()` decorator. It still works, and you'll meet it at work:

  | | `@Input()`: older code | `input()`: today's way, ours |
  |---|---|---|
  | declare | `@Input({ required: true }) pizza!: MenuPizza;` | `readonly pizza = input.required<MenuPizza>();` |
  | read | `pizza.name` | `pizza().name` |
  | a value made from it | `ngOnChanges` or a setter | `computed(() => this.pizza().ingredients.length)` |
  | the card changes it | possible: `this.pizza = …` | not possible: it's read-only |

### 🔗 Links and a 404 page
```html
<a matButton routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">
  Menu
</a>
<a matButton routerLink="/pizza-maker" routerLinkActive="active">Make a pizza</a>
```
```ts
// app.routes.ts: the last route
{
  path: '**',
  title: 'Page not found - Pizza App',
  loadComponent: () => import('./core/layout/not-found/not-found').then((m) => m.NotFound),
},
```
- 🔗 **`routerLink="/"` instead of `href="/"`:** `href` loads the whole app again from the server, `routerLink` only swaps the page.
- 📥 **`RouterLink` and `RouterLinkActive` go into the component's `imports`**, like everything else the template uses.
- ✨ **`routerLinkActive="active"`** adds the CSS class while the link's URL is open. The Menu link needs `[routerLinkActiveOptions]="{ exact: true }"`: every URL starts with `/`, so without it Menu would always look active.
- 🌟 **`**` matches every URL:** the router tries the routes from top to bottom, so it goes last.

---

### 🍕 The pizza maker: state as signals
Everything the user picks is a signal, and the price follows by itself:
```ts
export class PizzaMaker {
  protected readonly name = signal('Custom pizza');
  protected readonly size = signal<PizzaSize>('Medium');
  protected readonly ingredients = signal<Ingredient[]>([]);

  protected readonly price = computed(() => calculatePizzaPrice(this.size(), this.ingredients()));

  protected onNameInput(event: Event): void {
    const nameInput = event.target as HTMLInputElement;
    this.name.set(nameInput.value);
  }
}
```
```html
<input matInput [value]="name()" (input)="onNameInput($event)" />

<mat-button-toggle-group [value]="size()" (change)="size.set($event.value)">
  @for (sizeOption of sizes; track sizeOption) {
    <mat-button-toggle [value]="sizeOption">{{ sizeOption }}</mat-button-toggle>
  }
</mat-button-toggle-group>
```
- 📨 **`$event`** is what the event carries: for `(input)` the browser's `Event`, for a component's own event the value it sent (see [output()](#output)).
- 🎭 **`as HTMLInputElement` is a type assertion:** *"I know this target is an `<input>`"*. Like a cast in C#, but only for the compiler: nothing is checked while the app runs.
- 📖 **`Record<PizzaSize, number>`** (in `pizza-price.ts`) is a dictionary with a value for **every** size: `{ Small: 1, Medium: 3, Large: 5 }`. Leave one out and it doesn't compile.
- 🔄 **_Start over_** sets the three signals back to their starting values, and the whole page follows.

### 🔁 model(): two-way binding
The `IngredientPicker` changes a list that its parent owns:
```ts
export class IngredientPicker {
  protected readonly ingredients = INGREDIENTS;
  readonly selectedIngredients = model<Ingredient[]>([]); // an input that can also write back

  protected onSelectionChange(event: MatSelectionListChange): void {
    const values: Ingredient[] = event.source.selectedOptions.selected.map((option) => option.value);
    this.selectedIngredients.set(values); // the parent's signal changes too
  }
}
```
```html
<!-- pizza-maker.html: [( )], the "banana in a box" -->
<app-ingredient-picker [(selectedIngredients)]="ingredients" />
```
- ↔️ **Both ways:** the picker ticks what's in `ingredients`, and every tick goes back to `ingredients`. *Start over* empties the parent's list, and the checkboxes follow.
- 🍌 **`[(selectedIngredients)]="ingredients"`** is short for `[selectedIngredients]="ingredients()"` plus `(selectedIngredientsChange)="ingredients.set($event)"`. It takes the signal itself, without `()`.
- 🤔 **When:** a child that edits a value the parent owns (a picker, a toggle, a rating). Otherwise an input and an output are clearer.

<a id="output"></a>
### 📤 output(): events to the parent
```ts
export class PizzaCard {
  readonly pizza = input.required<MenuPizza>();
  readonly choose = output<MenuPizza>(); // what the card can tell its parent
}
```
```html
<!-- pizza-card.html -->
<button matButton="filled" type="button" (click)="choose.emit(pizza())">Make it</button>

<!-- home.html: $event is what the card emitted -->
<app-pizza-card [pizza]="pizza" (choose)="makePizza($event)" />
```
```ts
// home.ts
protected readonly router = inject(Router);

protected makePizza(selectedPizza: MenuPizza): void {
  // /pizza-maker?pizza=margherita
  this.router.navigate(['/pizza-maker'], { queryParams: { pizza: selectedPizza.id } });
}
```
- ⬇️⬆️ **Data goes down through inputs, events go up through outputs.** The card doesn't know what *Make it* does: `Home` decides, so the card stays simple.
- 💉 **`inject(Router)`** asks Angular for the router, like a constructor parameter in .NET. It works in a field, so there's no constructor at all.
- 🧭 **`router.navigate()`** is `routerLink` from code: for when something has to happen first.
- 📜 **Older code:** `@Output() choose = new EventEmitter<MenuPizza>();`. It still works.

### ⏱️ Lifecycle: the constructor vs ngOnInit
With `withComponentInputBinding()`, the router hands URL parameters to the page as inputs: `?pizza=margherita` fills `PizzaMaker`'s `pizza` input.
```ts
// app.config.ts
provideRouter(routes, withComponentInputBinding()),
```
```ts
export class PizzaMaker implements OnInit {
  readonly pizza = input<string>(); // 'margherita', or undefined

  constructor() {
    // too early: Angular creates the class first, and only then sets the inputs (undefined here)
  }

  ngOnInit(): void {
    // the inputs are set: prefill the pizza from the menu
    if (this.pizza()) {
      const menuPizza = findMenuPizza(this.pizza());
      if (menuPizza) {
        this.name.set(menuPizza.name);
        this.ingredients.set([...menuPizza.ingredients]);
      }
    }
  }
}
```

| | Runs | Good for |
|---|---|---|
| `constructor` | when Angular creates the component | `inject()`, the starting values of fields |
| `ngOnInit` | once, after the inputs got their first values | work that needs the inputs |
| `ngOnDestroy` | when the component is removed (another page opens) | cleaning up: timers, subscriptions |

- ✅ **`implements OnInit`** is an interface, as in C#. Optional, but a typo like `ngOnInt` then doesn't compile.
- 🔀 **Without `withComponentInputBinding()`** you'd `inject(ActivatedRoute)` and read its `queryParams` yourself. The input is shorter, and the page doesn't need to know about the router.
- 📋 **`[...menuPizza.ingredients]`** copies the array, so ticking ingredients in the maker never changes the menu.

### 🗂️ Where things go
```
src/app/
├── core/
│   └── layout/        header/, not-found/
├── shared/
│   └── models/        pizza-models.ts, pizza-size.ts
└── features/
    ├── menu/          menu-data.ts, home/, pizza-card/
    └── pizza-maker/   pizza-price.ts, pizza-maker/, ingredient-picker/
```

---

<a id="homework"></a>
## 🏠 Homework
None today. Make sure *Make it* opens the pizza maker on your computer: class 8 builds on it.

## ✅ By the end of the class
- 🍕 *Make it* on a menu card opens `/pizza-maker?pizza=…` with that pizza's name and ingredients
- 💶 the price follows the size and the ingredients, and *Start over* empties the pizza maker
- 🧭 *Menu* and *Make a pizza* are highlighted on their own pages, and an unknown URL shows *Page not found*
- 🔍 `ng build` shows no warnings, with `home`, `pizza-maker` and `not-found` among the *Lazy chunk files*

---

🔜 **Next class:** first what we didn't get to today: the cart. A `CartService` shared by the whole app with `@Service()` and `inject()`, the cart badge in the header, `effect()` so the cart survives a page reload, and the `Checkout` page. Then the display helpers: the built-in `currency` pipe, our own `ingredientName` pipe and a custom `HotPizza` directive.
