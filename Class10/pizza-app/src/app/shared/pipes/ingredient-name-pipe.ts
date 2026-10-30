import { Pipe, PipeTransform } from '@angular/core';
import { Ingredient } from '../models/pizza-models';

@Pipe({
  name: 'ingredientName',
})
export class IngredientNamePipe implements PipeTransform {
  transform(value: Ingredient | readonly Ingredient[]): string {
    if (Array.isArray(value)) {
      return value.map(ingredient => toReadableName(ingredient)).join(', ');
    }

    return toReadableName(value as Ingredient);
  }
}

// TomatoSauce => Tomato sauce
function toReadableName(ingredient: Ingredient): string {
  const words = ingredient.replace(/([a-z])([A-Z])/g, '$1 $2').toLowerCase();
  return words.charAt(0).toUpperCase() + words.slice(1);
}
