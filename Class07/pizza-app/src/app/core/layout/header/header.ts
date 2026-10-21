import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatToolbar } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  imports: [
    MatToolbar,
    MatIcon,
    RouterLink,
    RouterLinkActive
  ],
  selector: 'app-header',
  styleUrl: './header.scss',
  templateUrl: './header.html'
  // standalone: true,
})
export class Header {

}
// <app-header></app-header>