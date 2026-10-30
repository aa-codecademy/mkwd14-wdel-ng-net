import { computed, effect, Service, signal } from '@angular/core';
import { AddPizzaDto } from '../../shared/models/pizza-models';
import { PizzaSize } from '../../shared/models/pizza-size';

export interface CartItem extends AddPizzaDto {
    id: string;
    size?: PizzaSize;
}

const CART_KEY = 'pizza-app.cart';

@Service()
export class CartService {
    private readonly itemsState = signal<CartItem[]>(readSavedCart());

    constructor() {
        effect(() => {
            const jsonString = JSON.stringify(this.itemsState());
            localStorage.setItem(CART_KEY, jsonString);
        });
    }

    readonly items = this.itemsState.asReadonly();

    readonly count = computed(() => this.itemsState().length);

    readonly total = computed(() => {
        const sum = this.itemsState().reduce((total, item) => total + item.price, 0);
        return Math.round(sum * 100) / 100;
    });

    add(pizza: Omit<CartItem, 'id'>): void {
        this.itemsState.update(items => [...items, { ...pizza, id: crypto.randomUUID() }])
    }

    remove(id: string): void {
        this.itemsState.update(items => items.filter(item => item.id !== id));
    }

    clear(): void {
        this.itemsState.set([]);
    }
}

function readSavedCart(): CartItem[] {
    try {
        return JSON.parse(localStorage.getItem(CART_KEY) ?? '[]') as CartItem[];
    } catch {
        return [];
    }
}
