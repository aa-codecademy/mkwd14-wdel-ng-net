import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { PIZZA_SIZES, PizzaSize } from '../../../shared/models/pizza-size';
import { Ingredient } from '../../../shared/models/pizza-models';
import { calculatePizzaPrice } from '../pizza-price';
import { MatFormField, MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatChipSet, MatChip } from '@angular/material/chips';
import { MatInputModule } from '@angular/material/input';
import { IngredientPicker } from '../ingredient-picker/ingredient-picker';
import { findMenuPizza } from '../../menu/menu-data';
import { CartItem, CartService } from '../../../core/services/cart-service';
import { HotPizza } from '../../../shared/directives/hot-pizza';
import { IngredientNamePipe } from '../../../shared/pipes/ingredient-name-pipe';
import { PizzaService } from '../../../core/services/pizza-service';
import { NotificationService } from '../../../core/services/notification-service';
import { finalize } from 'rxjs';

const DEFAULT_NAME = 'Custom pizza';

@Component({
  imports: [
    MatFormField,
    MatLabel,
    MatButtonModule,
    MatButtonToggleModule,
    MatChipSet,
    MatChip,
    MatFormFieldModule,
    MatInputModule,
    IngredientPicker,
    HotPizza,
    IngredientNamePipe,
    CurrencyPipe
  ],
  selector: 'app-pizza-maker',
  styleUrl: './pizza-maker.scss',
  templateUrl: './pizza-maker.html',
})
export class PizzaMaker implements OnInit {

  readonly pizza = input<string>();
  private readonly cartService = inject(CartService);
  private readonly pizzaService = inject(PizzaService);
  private readonly notificationService = inject(NotificationService);

  protected readonly saving = signal(false);

  protected readonly sizes = PIZZA_SIZES;
  protected readonly name = signal(DEFAULT_NAME);
  protected readonly size = signal<PizzaSize>('Medium');
  protected readonly ingredients = signal<Ingredient[]>([]);

  protected readonly price = computed(() => calculatePizzaPrice(this.size(), this.ingredients()));

  // private readonly activeRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    // const pizzaQuery = this.activeRoute.queryParams
    // console.log(this.activeRoute.queryParams);
    console.log(this.pizza());
    if (this.pizza()) {
      const menuPizza = findMenuPizza(this.pizza());
      if (menuPizza) {
        this.name.set(menuPizza.name);
        this.ingredients.set([...menuPizza.ingredients]);
      }
    }

  }

  protected onNameInput(event: Event) {
    const nameInput = event.target as HTMLInputElement;
    this.name.set(nameInput.value)
  }

  protected reset(): void {
    this.name.set(DEFAULT_NAME);
    this.size.set('Medium');
    this.ingredients.set([]);
  }

  protected readonly isValid = computed(() => this.ingredients().length && this.name().trim().length)

  protected addToCart(): void {
    this.cartService.add(this.currentPizza());
  }

  protected addAndMakeAnother(): void {
    this.cartService.add(this.currentPizza());
    this.reset();
  }

  protected saveForLater(): void {
    this.saving.set(true);
    this.pizzaService
      .create(this.currentPizza())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe((savedPizza) => {
        this.notificationService.success(`${savedPizza.name} is saved in My pizzas.`);
      })
  }

  private currentPizza(): Omit<CartItem, 'id'> {
    return {
      name: this.name().trim(),
      description: `${this.size()} pizza`,
      price: this.price(),
      ingredients: this.ingredients(),
      size: this.size()
    }
  }
}
