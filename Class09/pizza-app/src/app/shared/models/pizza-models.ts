export const INGREDIENTS = [
    'TomatoSauce',
    'Mozzarella',
    'Ham',
    'Oregano',
    'Garlic',
    'Onion',
    'Mushrooms',
    'Parmesan',
    'Basil',
    'Pepperoni',
    'Egg',
    'ChilliPepper',
    'SourCream',
    'Bacon',
    'BlueCheese',
    'Gorgonzola',
    'Tuna',
    'Olives',
] as const;

export type Ingredient = (typeof INGREDIENTS)[number];

export interface PizzaDto {
    id: number;
    name: string;
    description: string | null;
    price: number;
    ingredients: Ingredient[];
    createdDate: string;
    updatedDate: string;
}

export interface AddPizzaDto {
    name: string;
    description?: string;
    price: number;
    ingredients: Ingredient[];
}
