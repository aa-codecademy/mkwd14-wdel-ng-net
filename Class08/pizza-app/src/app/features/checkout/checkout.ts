import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart-service';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CurrencyPipe, UpperCasePipe } from '@angular/common';
import { IngredientNamePipe } from '../../shared/pipes/ingredient-name-pipe';

@Component({
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
    CurrencyPipe,
    UpperCasePipe,
    IngredientNamePipe
  ],
  selector: 'app-checkout',
  styleUrl: './checkout.scss',
  templateUrl: './checkout.html',
})
export class Checkout {
  protected readonly cartService = inject(CartService);
}
