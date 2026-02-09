import { Component,Output,EventEmitter,Input,OnChanges,SimpleChanges} from '@angular/core';
import { FormBuilder,FormGroup,Validators } from '@angular/forms';
import {GroupService} from '../../services/group/group.service';
import {UserService} from '../../services/user/user.service';
import {User} from '../../interfaces/user';
import {Group} from '../../interfaces/group';
import {MatSnackBar} from '@angular/material/snack-bar';
import { switchMap,map } from 'rxjs/operators';
import { of,Observable } from 'rxjs';
@Component({
  selector: 'app-group-form',
  standalone: false,
  templateUrl: './group-form.component.html',
  styleUrl: './group-form.component.css'
})
export class GroupFormComponent implements OnChanges{
  @Input() group?: Group;
  @Input() allowAdminEdit: boolean = true;
  @Input() transparentBackground: boolean = false;
  groupForm: FormGroup;
  teamLeaders: User[] = [];
  @Output() groupCreatedId: EventEmitter<number> = new EventEmitter();
  constructor(private fb: FormBuilder
              ,private user_service: UserService
              ,private group_service: GroupService
              ,public snackBar: MatSnackBar ) {
    this.groupForm = this.fb.group(
      {
        name: ['', Validators.required],
        description: ['', [Validators.required,Validators.minLength(30),Validators.maxLength(1000)]],
        admin_ID: ['', [Validators.required,Validators.pattern(/^\d+$/)]]
      }
    )

    user_service.getTeamLeadsWithoutGroup().subscribe(
      {
        next: (users: User[]) => {
          this.teamLeaders = users;
        },
        error: (error) => console.log(error)
      }
    )
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['group'] && this.group) {
      this.groupForm.patchValue({
        name: this.group.name,
        description: this.group.description,
        admin_ID: this.group.admin_ID
      }, { emitEvent: false });

      this.user_service.getUser(this.group.admin_ID).subscribe(user => {

        this.teamLeaders = [...this.teamLeaders, user];

        this.groupForm.patchValue(
          { admin_ID: user.id },
          { emitEvent: false }
        );
      });
    }

    if (changes['allowAdminEdit']) {
      this.updateAdminControlState();
    }
  }


  onSubmit() {
    if (this.group) {
      const groupId = this.group.id;
      const adminId = this.groupForm.get('admin_ID')?.value;

      this.group_service
        .updateGroup({ id: groupId, ...this.groupForm.getRawValue() })
        .pipe(
          switchMap(() => {
            if (!adminId) return of(null);

            return this.userIsInGroup(adminId).pipe(
              switchMap((isInGroup) => {
                if (isInGroup) return of(null);
                return this.group_service.addUserToGroup(adminId, groupId);
              })
            );
          })
        )
        .subscribe({
          next: () => {
            this.snackBar.open('Group updated', 'Hide', { duration: 3000 });
          },
          error: (error) => {
            console.error('Error:', error);
            this.snackBar.open('Error updating group', 'Hide', { duration: 3000 });
          }
        });
      return;
    }


    const adminId = this.groupForm.get('admin_ID')?.value;

    this.group_service
      .createGroup(this.groupForm.value)
      .pipe(
        switchMap(response => {
          this.groupCreatedId.emit(response.id);
          if(adminId) {
            return this.group_service.addUserToGroup(adminId, response.id);
          }
          return of(response);
        })
      )
      .subscribe({
        next: () => {
          this.snackBar.open('Group created and admin added', 'Hide', { duration: 3000 });
        },
        error: (error) => {
          console.error('Error:', error);
          this.snackBar.open('Error creating group', 'Hide', { duration: 3000 });
        }
      });
  }
  private updateAdminControlState(){
    const ctrl = this.groupForm.get('admin_ID');
    if(!ctrl) return;
    if(this.allowAdminEdit){
      ctrl.enable({emitEvent:false});
    }else{
      ctrl.disable({emitEvent:false});
    }
  }

  private userIsInGroup(id: number): Observable<boolean> {
    return this.group_service.getUserGroups(id).pipe(
      map(groups => {
        if (!groups) return false;
        return groups.some(g => g.id === this.group?.id);
      })
    );
  }
}
