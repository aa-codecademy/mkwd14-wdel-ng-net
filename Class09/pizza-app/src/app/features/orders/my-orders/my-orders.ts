import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order-service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { OrderCard } from '../../../shared/ui/order-card/order-card';

@Component({
  imports: [
    RouterLink,
    MatProgressSpinnerModule,
    OrderCard
  ],
  selector: 'app-my-orders',
  styleUrl: './my-orders.scss',
  templateUrl: './my-orders.html',
})
export class MyOrders implements OnInit {
  protected readonly orderService = inject(OrderService);

  ngOnInit(): void {
    this.orderService.load();
  }

}
