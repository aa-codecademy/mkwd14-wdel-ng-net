import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        title: 'Menu - Pizza App',
        loadComponent: () => import('./features/menu/home/home').then(m => m.Home)
    },
    {
        path: 'pizza-maker',
        title: 'Make a pizza - Pizza App',
        loadComponent: () => import('./features/pizza-maker/pizza-maker/pizza-maker').then(m => m.PizzaMaker)
    },
    {
        path: 'checkout',
        title: 'Cart - Pizza App',
        loadComponent: () => import('./features/checkout/checkout').then(m => m.Checkout)
    },
    {
        path: '**',
        title: 'Page not found - Pizza App',
        loadComponent: () => import('./core/layout/not-found/not-found').then(m => m.NotFound)
    }
];
