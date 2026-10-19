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