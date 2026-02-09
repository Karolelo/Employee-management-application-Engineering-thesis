import { Component,Input,ViewChild,OnChanges,Output,EventEmitter,SimpleChanges } from '@angular/core';
import {UserListComponent} from '../user-list/user-list.component';
import {GroupService} from '../../services/group/group.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {forkJoin} from 'rxjs';
import {finalize} from 'rxjs/operators';
import {UserService} from '../../services/user/user.service';
import {pipe,map,filter} from 'rxjs'
@Component({
  selector: 'app-add-user-to-group-form',
  standalone: false,
  templateUrl: './add-user-to-group-form.component.html',
  styleUrl: './add-user-to-group-form.component.css'
})
export class AddUserToGroupFormComponent implements OnChanges {
  @Input() group_id: number = 0;
  @Output() usersAdded: EventEmitter<boolean> = new EventEmitter<boolean>()
  @Input() userIdToSkip!: number;

  @ViewChild(UserListComponent) userList!: UserListComponent;
  loading: boolean = false;

  existingUserIds: number[] =[];

  constructor(
    private group_service: GroupService,
    private user_service: UserService,
    private snackBar: MatSnackBar
  ) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['group_id'] && this.group_id) {
      this.user_service.getUsersFromGroup(this.group_id).pipe(
        map(users => users.map(user => user.id))
      ).subscribe((ids: number[]) => {
        this.existingUserIds = ids;
        this.userList.selectedIds = [...ids];
        this.userList.userIdToSkip = this.userIdToSkip;
      });
    }
  }
  onSubmit() {
    this.loading = true;

    const selectedIds = this.userList.selectedIds;

    /*if (selectedIds.length === 0) {
      this.snackBar.open('Choose at least one user', 'OK', { duration: 3000 });
      this.loading = false;
      return;
    }*/

    const toAdd = selectedIds.filter(id => !this.existingUserIds.includes(id));

    const toRemove = this.existingUserIds.filter(id => !selectedIds.includes(id));

    const addRequests = toAdd.map(id =>
      this.group_service.addUserToGroup(id, this.group_id)
    );

    const removeRequests = toRemove.map(id =>
      this.group_service.removeUserFromGroup(id, this.group_id)
    );

    const requests = [...addRequests, ...removeRequests];

    if (requests.length === 0) {
      this.loading = false;
      this.snackBar.open('No changes', 'OK', { duration: 2000 });
      return;
    }

    forkJoin(requests)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.usersAdded.emit(true);
        },
        error: (error) => {
          console.error('Error during updating group users:', error);
          this.snackBar.open(
            'Error during updating group users: ' + error.error?.message,
            'OK',
            { duration: 3000, panelClass: ['error-snackbar'] }
          );
        }
      });
  }
}
