import { Component, computed, input, OnInit, signal } from '@angular/core';
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
    IngredientPicker
  ],
  selector: 'app-pizza-maker',
  styleUrl: './pizza-maker.scss',
  templateUrl: './pizza-maker.html',
})
export class PizzaMaker implements OnInit {

  readonly pizza = input<string>();

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

}
