import { Component, inject } from '@angular/core';
import { MENU, MenuPizza } from '../menu-data';
import { NgOptimizedImage } from '@angular/common';
import { PizzaCard } from '../pizza-card/pizza-card';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  imports: [
    NgOptimizedImage,
    PizzaCard,
    MatButtonModule,
    RouterLink
  ],
  selector: 'app-home',
  styleUrl: './home.scss',
  templateUrl: './home.html',
})
export class Home {
  protected readonly menu = MENU;
  protected readonly router = inject(Router);

  // constructor(private router: Router) {

  // }

  protected makePizza(selectedPizza: MenuPizza): void {
    console.log(selectedPizza);
    // /pizza-maker?pizza=<selectedPizza.id>
    this.router.navigate(['/pizza-maker'], { queryParams: { pizza: selectedPizza.id } })
  }
}
