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
export class AdminDashboardComponent implements OnInit{
  private breakpointObserver = inject(BreakpointObserver);
  private userService: UserService = inject(UserService);
  public userCount = 0;
  private router: Router = inject(Router);
  private userStore_service = inject(UserStoreService)
  private group_service = inject(GroupService)
  private matSnackBar = inject(MatSnackBar)
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
    const id = this.userStore_service.getUserId();
    if(id) {
      this.group_service.getGroupByAdminId(id).subscribe(
        {
          next: (group)=> {this.router.navigate([`dashboard/groupsManage/${group.id}`])},
          error: (error: any) => {
            if(error.status===404){
              this.matSnackBar.open('You are not admin of any group', '', {duration: 3000})
            }
            else {
              this.matSnackBar.open('Error during finding your group', 'Ok', {duration: 3000})
            }
          }
        }
      )
    }
  }

  onSeeGroupsClick() {
    this.router.navigate(['dashboard/groups']);
  }

  onAddGroupClick() {
    this.router.navigate(['dashboard/group/create']);
  }

}
