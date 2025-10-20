import { Component, inject,OnInit } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { map } from 'rxjs/operators';
import {UserService} from '../../services/user.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css',
  standalone: false
})
export class AdminDashboardComponent implements OnInit{
  private breakpointObserver = inject(BreakpointObserver);
  private userService: UserService = inject(UserService);
  public userCount = 0;
  private router: Router = inject(Router);
  ngOnInit(): void {
    this.userService.getUsers().subscribe(
      (data) => {
        this.userCount = data.length;
      })
  }
  /** Based on the screen size, switch from standard to one column per row */
  cards = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map(({ matches }) => {
      if (matches) {
        return [
          { title: 'Card 1', cols: 1, rows: 1,type: 'userStats' },
          { title: 'Card 2', cols: 1, rows: 1,type: 'taskStats' },
          { title: 'Card 3', cols: 1, rows: 1,type: 'groupsStats' },
          { title: 'Card 4', cols: 1, rows: 1,type: 'other' }
        ];
      }

      return [
        { title: 'Users Managment', cols: 2, rows: 1, type: 'userStats' },
        { title: 'Monthly task info', cols: 1, rows: 1, type: 'taskStats' },
        { title: 'Group Managment', cols: 1, rows: 2, type: 'groupStats'},
        { title: 'Card 4', cols: 1, rows: 1, type: 'other' }
      ];
    })
  );

  onManageUserClick() {
    this.router.navigate(['dashboard/users']);
  }

  onManageMyGroupClick() {

  }

  onSeeGroupsClick() {

  }

  onAddGroupClick() {

  }

  onEditGroupClick() {

  }

  onDeleteGroupClick() {

  }
}
//Fajny sposób na dodanie menu w math components
/*
<!-- <button mat-icon-button class="more-button" [matMenuTriggerFor]="menu" aria-label="Toggle menu">
  <mat-icon>more_vert</mat-icon>
  </button>
  <mat-menu #menu="matMenu" xPosition="before">
  <button mat-menu-item>Expand</button>
  <button mat-menu-item>Remove</button>
</mat-menu>-->*/
