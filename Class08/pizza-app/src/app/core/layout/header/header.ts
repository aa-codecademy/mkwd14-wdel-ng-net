import { Component, inject } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule, MatIconButton } from '@angular/material/button';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatToolbar, MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CartService } from '../../services/cart-service';

@Component({
  imports: [
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatBadgeModule,
    MatIconButton
  ],
  selector: 'app-header',
  styleUrl: './header.scss',
  templateUrl: './header.html'
  // standalone: true,
})
export class Header {
  protected readonly cartService = inject(CartService);
}
// <app-header></app-header>