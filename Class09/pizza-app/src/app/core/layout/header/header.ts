import { Component, inject } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule, MatIconButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CartService } from '../../services/cart-service';
import { AuthService } from '../../services/auth-service';

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
})
export class Header {
  protected readonly cartService = inject(CartService);
  protected readonly authService = inject(AuthService);

  protected logout(): void {
    this.authService.logout();
  }
}