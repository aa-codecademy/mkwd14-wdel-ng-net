import { Component, input } from '@angular/core';
import { MenuPizza } from '../menu-data';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent } from '@angular/material/card';
import { MatChipSet, MatChip } from '@angular/material/chips';
import { NgOptimizedImage } from '@angular/common';

@Component({
  imports: [
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent,
    MatChipSet,
    MatChip,
    NgOptimizedImage
  ],
  selector: 'app-pizza-card',
  styleUrl: './pizza-card.scss',
  templateUrl: './pizza-card.html',
})
export class PizzaCard {
  // @Input({ required: true }) pizza: MenuPizza = null!;
  readonly pizza = input.required<MenuPizza>();
}
