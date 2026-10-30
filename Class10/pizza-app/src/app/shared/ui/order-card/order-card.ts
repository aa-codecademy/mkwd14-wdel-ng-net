import { Component, input } from '@angular/core';
import { OrderDto } from '../../models/order-models';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { IngredientNamePipe } from '../../pipes/ingredient-name-pipe';

@Component({
  imports: [
    CurrencyPipe,
    DatePipe,
    MatCardModule,
    IngredientNamePipe
  ],
  selector: 'app-order-card',
  styleUrl: './order-card.scss',
  templateUrl: './order-card.html',
})
export class OrderCard {
  readonly order = input.required<OrderDto>();
  readonly showUser = input(false);
}
