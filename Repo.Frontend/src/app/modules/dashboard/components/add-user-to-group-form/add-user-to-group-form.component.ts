import { Component,Input,ViewChild,AfterViewInit,Output,EventEmitter } from '@angular/core';
import {UserListComponent} from '../user-list/user-list.component';
import {GroupService} from '../../services/group/group.service';
import {Router} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';
import {forkJoin} from 'rxjs';
import {finalize} from 'rxjs/operators';
import {ProgressBarMode,MatProgressBar} from '@angular/material/progress-bar';
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
  @Output() usersAdded: EventEmitter<boolean> = new EventEmitter<boolean>()
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

    if (this.userList.selectedIds.length === 0) {
      this.snackBar.open('Wybierz przynajmniej jednego użytkownika', 'OK', {
        duration: 3000
      });
      this.loading = false;
      return;
    }

    const requests = this.userList.selectedIds
      .map(id => this.group_service.addUserToGroup(id,this.group_id));

    console.log(this.userList.selectedIds)
    console.log(this.group_id)

    forkJoin(requests)
      .pipe(
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: () => {
          this.usersAdded.emit(true);
          //this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          console.error('Error during adding user:', error);
          this.snackBar.open(
            'Error during adding users to group: '+error.error.message,
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
