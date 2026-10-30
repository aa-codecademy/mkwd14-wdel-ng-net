import { Component, inject, OnInit } from '@angular/core';
import { PizzaService } from '../../../core/services/pizza-service';
import { CartService } from '../../../core/services/cart-service';
import { NotificationService } from '../../../core/services/notification-service';
import { CurrencyPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { HotPizza } from '../../../shared/directives/hot-pizza';
import { IngredientNamePipe } from '../../../shared/pipes/ingredient-name-pipe';
import { PizzaDto } from '../../../shared/models/pizza-models';

@Component({
  imports: [
    CurrencyPipe,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    HotPizza,
    IngredientNamePipe,
  ],
  selector: 'app-my-pizzas',
  styleUrl: './my-pizzas.scss',
  templateUrl: './my-pizzas.html',
})
export class MyPizzas implements OnInit {
  protected readonly pizzaService = inject(PizzaService);
  protected readonly cartService = inject(CartService);
  protected readonly notificationService = inject(NotificationService);
  
  ngOnInit(): void {
    this.pizzaService.load();
  }

  addToCart(pizza: PizzaDto) : void {
    this.cartService.add({
      name: pizza.name,
      description: pizza.description ?? undefined,
      price: pizza.price,
      ingredients: pizza.ingredients
    });

    this.notificationService.success(`${pizza.name} is in the cart.`)
  }
}
