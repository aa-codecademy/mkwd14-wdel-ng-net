import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order-service';
import { OrderCard } from '../../../shared/ui/order-card/order-card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  imports: [
    OrderCard,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  selector: 'app-admin-orders',
  styleUrl: './admin-orders.scss',
  templateUrl: './admin-orders.html',
})
export class AdminOrders implements OnInit {
  protected readonly orderService = inject(OrderService);
  
  ngOnInit(): void {
    this.orderService.load();
  }

}
