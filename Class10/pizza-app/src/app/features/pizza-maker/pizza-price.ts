import { Ingredient } from "../../shared/models/pizza-models";
import { PizzaSize } from "../../shared/models/pizza-size";

const BASE_PRICE: Record<PizzaSize, number> = { Small: 1, Medium: 3, Large: 5 }

const INGREDIENT_PRICE: Record<PizzaSize, number> = { Small: 0.15, Medium: 0.3, Large: 0.45 }

export function calculatePizzaPrice(size: PizzaSize, ingredients: Ingredient[]): number {
    const price = BASE_PRICE[size] + ingredients.length * INGREDIENT_PRICE[size];
    return Math.round(price * 100) / 100;
}