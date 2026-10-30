import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { adminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
    {
        path: '',
        title: 'Menu - Pizza App',
        loadComponent: () => import('./features/menu/home/home').then(m => m.Home)
    },
    {
        path: 'login',
        title: 'Log in - Pizza App',
        loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
    },
    {
        path: 'pizza-maker',
        title: 'Make a pizza - Pizza App',
        canActivate: [authGuard],
        loadComponent: () => import('./features/pizza-maker/pizza-maker/pizza-maker').then(m => m.PizzaMaker)
    },
    {
        path: 'checkout',
        title: 'Cart - Pizza App',
        canActivate: [authGuard],
        loadComponent: () => import('./features/checkout/checkout').then(m => m.Checkout)
    },
    {
        path: 'my-pizzas',
        title: 'My pizzas - Pizza App',
        canActivate: [authGuard],
        loadComponent: () => import('./features/my-pizzas/my-pizzas/my-pizzas').then((m) => m.MyPizzas),
    },
    {
        path: 'orders',
        title: 'My orders - Pizza App',
        canActivate: [authGuard],
        loadComponent: () => import('./features/orders/my-orders/my-orders').then((m) => m.MyOrders),
    },
    {
        path: 'admin',
        canActivate: [authGuard, adminGuard],
        children: [
            {
                path: '',
                redirectTo: 'orders',
                pathMatch: 'full'
            },
            {
                path: 'orders',
                title: 'All orders - Pizza App',
                loadComponent: () => import('./features/admin/admin-orders/admin-orders').then(m => m.AdminOrders)
            },
            {
                path: 'users',
                title: 'Users - Pizza App',
                loadComponent: () => import('./features/admin/admin-users/admin-users').then(m => m.AdminUsers)
            }
        ]
    },
    {
        path: '**',
        title: 'Page not found - Pizza App',
        loadComponent: () => import('./core/layout/not-found/not-found').then(m => m.NotFound)
    }
];
