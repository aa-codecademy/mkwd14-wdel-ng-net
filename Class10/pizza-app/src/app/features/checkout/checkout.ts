import { Component, inject, signal } from '@angular/core';
import { CartService } from '../../core/services/cart-service';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CurrencyPipe, UpperCasePipe } from '@angular/common';
import { IngredientNamePipe } from '../../shared/pipes/ingredient-name-pipe';
import { OrderService } from '../../core/services/order-service';
import { NotificationService } from '../../core/services/notification-service';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import as from '@angular/common/locales/as';
import { AddPizzaDto } from '../../shared/models/pizza-models';
import { AddOrderDto } from '../../shared/models/order-models';
import { finalize } from 'rxjs';

@Component({
  imports: [
    CurrencyPipe,
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    IngredientNamePipe,
    UpperCasePipe
  ],
  selector: 'app-checkout',
  styleUrl: './checkout.scss',
  templateUrl: './checkout.html',
})
export class Checkout {
  protected readonly cartService = inject(CartService);
  protected readonly orderService = inject(OrderService);
  protected readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(NonNullableFormBuilder);
  protected readonly placing = signal(false);

  protected readonly checkoutForm = this.formBuilder.group({
    addressTo: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.maxLength(500)]
  })

  protected placeOrder(): void {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    const { addressTo, description } = this.checkoutForm.getRawValue();

    const pizzaCartItems: AddPizzaDto[] = this.cartService.items()
      .map(item => ({
        name: item.name,
        description: item.description,
        price: item.price,
        ingredients: item.ingredients
      } as AddPizzaDto))

    const order: AddOrderDto = {
      addressTo,
      description: description || undefined,
      pizzas: pizzaCartItems
    }
    this.placing.set(true);

    this.orderService
      .create(order)
      .pipe(finalize(() => this.placing.set(false)))
      .subscribe(createdOrder => {
        this.cartService.clear();
        this.notificationService.success(`Order #${createdOrder.id} placed. Total $${createdOrder.totalPrice.toFixed(2)}`);
        this.router.navigate(['/orders'])
      });
  }

}
