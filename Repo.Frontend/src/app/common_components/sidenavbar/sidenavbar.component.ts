import { Component } from '@angular/core';

@Component({
  selector: 'app-sidenavbar',
  standalone: false,
  templateUrl: './sidenavbar.component.html',
  styleUrl: './sidenavbar.component.css'
})
export class SidenavbarComponent {
  menuItems = [
    { link: '/dashboard', icon: 'dashboard' },
    { link: '/profile', icon: 'account_circle' },
    { link: '/tasks', icon: 'assignment' },
    { link: '/calendar', icon: 'today' },
  ];
}
