import { Component, inject,OnInit } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import {UserService} from '../../services/user/user.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {GroupService} from '../../services/group/group.service';
import {MatSnackBar} from'@angular/material/snack-bar'
@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css',
  standalone: false
})
export class AdminDashboardComponent implements OnInit {
  private readonly HANDSET_CARDS = [
    { title: 'Card 1', cols: 1, rows: 1, type: 'userStats' },
    { title: 'Card 2', cols: 1, rows: 1, type: 'taskStats' },
    { title: 'Card 3', cols: 1, rows: 1, type: 'groupsStats' },
    { title: 'Card 4', cols: 1, rows: 1, type: 'other' }
  ];

  private readonly DESKTOP_CARDS = [
    { title: 'Users Managment', cols: 2, rows: 1, type: 'userStats' },
    { title: 'Monthly task info', cols: 1, rows: 1, type: 'taskStats' },
    { title: 'Group Managment', cols: 1, rows: 2, type: 'groupStats' },
    { title: 'Card 4', cols: 1, rows: 1, type: 'other' }
  ];
  //Errors types
  private readonly ADMIN_ROLE = 'Admin';
  private readonly UNAUTHORIZED_MESSAGE = 'U cant create manage users as team leader';
  private readonly UNAUTHORIZED_GROUP_MESSAGE = 'U cant create new group as teamLeader';
  private readonly NOT_GROUP_LEADER_MESSAGE = 'You are not leader of any group';
  private readonly GROUP_FETCH_ERROR_MESSAGE = 'Error during finding your group';

  private breakpointObserver = inject(BreakpointObserver);
  private userService = inject(UserService);
  private router = inject(Router);
  private userStoreService = inject(UserStoreService);
  private groupService = inject(GroupService);
  private matSnackBar = inject(MatSnackBar);

  public userCount = 0;
  public groupCount = 0;
  ngOnInit(): void {
    this.userService.getUsers().subscribe((data) => {
      if(data)
        this.userCount = data.length
    });
    this.groupService.getGroups().subscribe((data)=> {
      if(data)
        this.groupCount = data.length
    })
  }

  /** Based on the screen size, switch from standard to one column per row */
  cards = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map(({ matches }) => matches ? this.HANDSET_CARDS : this.DESKTOP_CARDS)
  );

  onManageUserClick(): void {
    if (this.checkAdminRoleOrShowError(this.UNAUTHORIZED_MESSAGE)) {
      this.router.navigate(['dashboard/users']);
    }
  }

  onManageMyGroupClick(): void {
    const userId = this.userStoreService.getUserId();
    if (userId) {
      this.navigateToGroupManagement(userId);
    }
  }

  onSeeGroupsClick(): void {
    this.router.navigate(['dashboard/groups']);
  }

  onAddGroupClick(): void {
    if (this.checkAdminRoleOrShowError(this.UNAUTHORIZED_GROUP_MESSAGE)) {
      this.router.navigate(['dashboard/group/create']);
    }
  }

  private checkAdminRoleOrShowError(errorMessage: string): boolean {
    if (this.userStoreService.hasRole(this.ADMIN_ROLE)) {
      return true;
    }
    this.showSnackBarMessage(errorMessage);
    return false;
  }

  private navigateToGroupManagement(userId: number): void {
    this.groupService.getGroupByAdminId(userId).subscribe({
      next: (group) => {
        this.router.navigate([`dashboard/groupsManage/${group.id}`]);
      },
      error: (error) => {
        const message = error.status === 404
          ? this.NOT_GROUP_LEADER_MESSAGE
          : this.GROUP_FETCH_ERROR_MESSAGE;
        this.showSnackBarMessage(message);
      }
    });
  }

  private showSnackBarMessage(message: string, action: string = 'Ok', duration: number = 3000): void {
    this.matSnackBar.open(message, action, { duration });
  }
}
