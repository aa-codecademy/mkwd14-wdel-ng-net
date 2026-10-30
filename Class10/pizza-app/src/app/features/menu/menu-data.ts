import { Ingredient } from "../../shared/models/pizza-models";

export interface MenuPizza {
    id: string;
    name: string;
    description: string;
    image: string;
    imageWidth: number;
    imageHeight: number;
    ingredients: Ingredient[];
}

export const MENU: MenuPizza[] = [
    {
        id: 'margherita',
        name: 'Margherita',
        description: 'A classic with tomato sauce and mozzarella on a crispy crust.',
        image: 'images/margherita.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['TomatoSauce', 'Mozzarella'],
    },
    {
        id: 'neapolitan',
        name: 'Neapolitan',
        description: 'Traditional pizza with tomato sauce, mozzarella and savory ham.',
        image: 'images/neapolitan.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['TomatoSauce', 'Mozzarella', 'Ham'],
    },
    {
        id: 'quattro-formaggi',
        name: 'Quattro Formaggi',
        description: 'A rich blend of four cheeses: parmesan, mozzarella, blue cheese and gorgonzola.',
        image: 'images/quatro-formagi.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['Parmesan', 'Mozzarella', 'BlueCheese', 'Gorgonzola'],
    },
    {
        id: 'bacon',
        name: 'Bacon',
        description: 'A tasty combination of crispy bacon, tomato sauce and mozzarella.',
        image: 'images/bacon.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['Bacon', 'TomatoSauce', 'Mozzarella'],
    },
    {
        id: 'bianca',
        name: 'Bianca',
        description: 'A simple, creamy pizza topped with rich sour cream.',
        image: 'images/bianca.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['SourCream'],
    },
    {
        id: 'capricciosa',
        name: 'Capricciosa',
        description: 'A savory mix of ham, mushrooms and mozzarella on tomato sauce.',
        image: 'images/capri.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['Ham', 'TomatoSauce', 'Mushrooms', 'Mozzarella'],
    },
    {
        id: 'mexicana',
        name: 'Mexicana',
        description: 'A spicy pizza with gorgonzola, pepperoni, olives and chilli peppers.',
        image: 'images/mexicana.png',
        imageWidth: 510,
        imageHeight: 510,
        ingredients: ['TomatoSauce', 'Gorgonzola', 'Olives', 'Pepperoni', 'ChilliPepper'],
    },
    {
        id: 'pepperoni',
        name: 'Pepperoni',
        description: 'A classic pepperoni pizza with gorgonzola and tomato sauce.',
        image: 'images/pepperoni.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['TomatoSauce', 'Gorgonzola', 'Pepperoni'],
    },
    {
        id: 'tuna',
        name: 'Tuna',
        description: 'Tuna, onion and mozzarella on a tomato sauce base.',
        image: 'images/tuna.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['TomatoSauce', 'Tuna', 'Mozzarella', 'Onion'],
    },
    {
        id: 'vegetariana',
        name: 'Vegetariana',
        description: 'A veggie pizza with olives, mushrooms and mozzarella on tomato sauce.',
        image: 'images/vegetariana.png',
        imageWidth: 510,
        imageHeight: 392,
        ingredients: ['TomatoSauce', 'Mozzarella', 'Olives', 'Mushrooms'],
    },
];

export function findMenuPizza(id: string | undefined): MenuPizza | undefined {
    return MENU.find(pizza => pizza.id === id);
}