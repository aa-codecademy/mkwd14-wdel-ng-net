import { Component } from '@angular/core';
import { MENU } from '../menu-data';
import { NgOptimizedImage } from '@angular/common';
import { PizzaCard } from '../pizza-card/pizza-card';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  imports: [
    NgOptimizedImage,
    PizzaCard,
    RouterLink,
    MatButtonModule
  ],
  selector: 'app-home',
  styleUrl: './home.scss',
  templateUrl: './home.html',
})
export class Home {
  protected readonly menu = MENU;
}
