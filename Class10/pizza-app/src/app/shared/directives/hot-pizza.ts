import { computed, Directive, input } from '@angular/core';
import { Ingredient } from '../models/pizza-models';

@Directive({
  selector: '[appHotPizza]',
  host: { '[class.hot]': 'isHot()' }
})
export class HotPizza {
  readonly ingredients = input<readonly Ingredient[]>([], { alias: 'appHotPizza' });

  protected readonly isHot = computed(() => this.ingredients().includes('ChilliPepper'))
}
