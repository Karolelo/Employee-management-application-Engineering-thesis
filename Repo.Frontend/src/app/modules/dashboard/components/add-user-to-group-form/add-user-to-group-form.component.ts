import { Component,Input,ViewChild,AfterViewInit } from '@angular/core';
import {UserListComponent} from '../user-list/user-list.component';
import {GroupService} from '../../services/group/group.service';
import {Router} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';
import {forkJoin} from 'rxjs';
import {finalize} from 'rxjs/operators';
@Component({
  selector: 'app-add-user-to-group-form',
  standalone: false,
  templateUrl: './add-user-to-group-form.component.html',
  styleUrl: './add-user-to-group-form.component.css'
})
export class AddUserToGroupFormComponent implements AfterViewInit {
  @Input() group_id: number = 0;
  @ViewChild(UserListComponent) userList!: UserListComponent;
  loading: boolean = false;
  constructor(
    private group_service: GroupService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngAfterViewInit() {
    setTimeout(() => {
      this.userList.dataSource.filterExistingGroupUsers(this.group_id);
    });
  }

  onSubmit() {
    this.loading = true;

    const requests = this.userList.selectedIds
      .map(id => this.group_service.addUserToGroup(this.group_id, id));

    forkJoin(requests)
      .pipe(
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          console.error('Error during adding user:', error);
          this.snackBar.open(
            'Error during adding users to group:',
            'OK',
            {
              duration: 3000,
              panelClass: ['error-snackbar']
            }
          );
        }
      });
  }
}
