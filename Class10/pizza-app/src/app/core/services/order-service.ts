import { HttpClient } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AddOrderDto, ChangeOrderStatusDto, OrderDto, OrderStatus } from '../../shared/models/order-models';
import { finalize, Observable } from 'rxjs';

@Service()
export class OrderService {
    private readonly http = inject(HttpClient);
    private readonly url = `${environment.apiUrl}/orders`;

    private readonly ordersState = signal<OrderDto[]>([]);
    readonly orders = this.ordersState.asReadonly();

    private readonly loadingState = signal(false);
    readonly loading = this.loadingState.asReadonly();

    load(): void {
        this.ordersState.set([]);
        this.loadingState.set(true);
        this.http
            .get<OrderDto[]>(this.url)
            .pipe(finalize(() => {
                this.loadingState.set(false);
            }))
            .subscribe(orders => this.ordersState.set(orders));
    }

    create(order: AddOrderDto): Observable<OrderDto> {
        return this.http.post<OrderDto>(this.url, order);
    }

    changeStatus(id: number, status: OrderStatus): void {
        const body: ChangeOrderStatusDto = { status };
        this.http
            .patch<OrderDto>(`${this.url}/${id}/status`, body)
            .subscribe(changedOrder => {
                this.ordersState.update(orders => orders.map(order => order.id === changedOrder.id ? changedOrder : order));
            });
    }
}
