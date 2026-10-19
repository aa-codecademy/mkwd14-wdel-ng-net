import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        title: 'Menu - Pizza App',
        loadComponent: () => import('./features/menu/home/home').then(m => m.Home)
    }
];
