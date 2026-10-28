import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AddOrderDto, OrderDto } from '../../shared/models/order-models';
import { Observable } from 'rxjs';

@Service()
export class OrderService {
    private readonly http = inject(HttpClient);
    private readonly url = `${environment.apiUrl}/orders`

    create(order: AddOrderDto): Observable<OrderDto> {
        return this.http.post<OrderDto>(this.url, order);
    }
}
