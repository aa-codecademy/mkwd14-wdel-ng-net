import { Component, model, signal } from '@angular/core';
import { Ingredient, INGREDIENTS } from '../../../shared/models/pizza-models';
import { MatListModule, MatSelectionListChange } from '@angular/material/list';
import { IngredientNamePipe } from '../../../shared/pipes/ingredient-name-pipe';

@Component({
  imports: [MatListModule, IngredientNamePipe],
  selector: 'app-ingredient-picker',
  styleUrl: './ingredient-picker.scss',
  templateUrl: './ingredient-picker.html',
})
export class IngredientPicker {
  protected readonly ingredients = INGREDIENTS;
  readonly selectedIngredients = model<Ingredient[]>([]);

  protected onSelectionChange(event: MatSelectionListChange) {
    const values: Ingredient[] = event.source.selectedOptions.selected.map(option => option.value);
    this.selectedIngredients.set(values);
  }
}
