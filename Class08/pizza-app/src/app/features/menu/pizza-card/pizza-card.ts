import { Component, input, output } from '@angular/core';
import { MenuPizza } from '../menu-data';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatChipSet, MatChip } from '@angular/material/chips';
import { NgOptimizedImage } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { HotPizza } from '../../../shared/directives/hot-pizza';
import { IngredientNamePipe } from '../../../shared/pipes/ingredient-name-pipe';

@Component({
  imports: [
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent,
    MatChipSet,
    MatChip,
    NgOptimizedImage,
    MatCardActions,
    MatButton,
    HotPizza,
    IngredientNamePipe
  ],
  selector: 'app-pizza-card',
  styleUrl: './pizza-card.scss',
  templateUrl: './pizza-card.html',
})
export class PizzaCard {
  // @Input({ required: true }) pizza: MenuPizza = null!;
  readonly pizza = input.required<MenuPizza>();
  readonly choose = output<MenuPizza>();
}
