import { AddPizzaDto, PizzaDto } from "./pizza-models";

export type OrderStatus = 'Pending' | 'Preparing' | 'Delivered' | 'Cancelled' | 'Returned';

export interface OrderDto {
    id: number;
    addressTo: string;
    description: string | null;
    totalPrice: number;
    status: OrderStatus;
    userId: string;
    userName: string | null;
    pizzas: PizzaDto[];
    createdDate: string;
    updatedDate: string;
}

export interface AddOrderDto {
    addressTo: string;
    description?: string;
    pizzas: AddPizzaDto[];
}

export interface ChangeOrderStatusDto {
    status: OrderStatus;
}