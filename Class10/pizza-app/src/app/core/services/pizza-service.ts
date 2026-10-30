import { HttpClient } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AddPizzaDto, PizzaDto } from '../../shared/models/pizza-models';
import { finalize, Observable } from 'rxjs';

@Service()
export class PizzaService {
    private readonly http = inject(HttpClient);
    private readonly url = `${environment.apiUrl}/pizzas`;

    private readonly pizzasState = signal<PizzaDto[]>([]);
    readonly pizzas = this.pizzasState.asReadonly();

    private readonly loadingState = signal(false);
    readonly loading = this.loadingState.asReadonly();

    load(): void {
        this.pizzasState.set([]);
        this.loadingState.set(true);
        this.http
            .get<PizzaDto[]>(this.url)
            .pipe(finalize(() => this.loadingState.set(false)))
            .subscribe(pizzas => this.pizzasState.set(pizzas));
    }

    create(pizza: AddPizzaDto): Observable<AddPizzaDto> {
        return this.http.post<AddPizzaDto>(this.url, pizza);
    }

    delete(id: number): void {
        this.http
            .delete<void>(`${this.url}/${id}`)
            .subscribe(() => this.pizzasState.update(pizzas => pizzas.filter(pizza => pizza.id !== id)))
    }
}
