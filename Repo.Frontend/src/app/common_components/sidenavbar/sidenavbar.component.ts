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
    { link: '/tasks', icon: 'assignment' },
    { link: '/tasks/gantt',icon: 'view_timeline'},
    { link: '/calendar', icon: 'today' },
    { link: '/grades',icon: 'analytics'},
    { link: '/worktime',icon: 'next_week'},
    { link: '/myGroups',icon: 'group'}
  ];
}
