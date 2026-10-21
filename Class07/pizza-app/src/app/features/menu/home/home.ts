import { Component, computed, signal } from '@angular/core';
import { MENU } from '../menu-data';
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
  selector: 'app-home',
  styleUrl: './home.scss',
  templateUrl: './home.html',
})
export class Home {
  protected readonly menu = MENU;
}

// export class Home {
//   protected readonly shopName = 'Pizza App';
//   protected readonly toppings = ['Tomato sauce', 'Mozzarella', 'Basil'];
//   protected readonly pizzaCount = signal(0);
//   protected readonly seconds = signal(0);

//   protected readonly total = computed(() => this.pizzaCount() * 5);

//   constructor() {
//     setInterval(() => {
//       this.seconds.update(count => count + 1);
//     }, 1000);
//   }

//   protected addPizza(): void {
//     this.pizzaCount.update(count => count + 1);
//   }

//   protected reset(): void {
//     this.pizzaCount.set(0);
//   }
// }
