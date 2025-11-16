import { Component,Output,EventEmitter,Input,OnChanges,SimpleChanges} from '@angular/core';
import { FormBuilder,FormGroup,Validators } from '@angular/forms';
import {GroupService} from '../../services/group/group.service';
import {UserService} from '../../services/user/user.service';
import {User} from '../../interfaces/user';
import {Group} from '../../interfaces/group';
import {MatSnackBar} from '@angular/material/snack-bar';
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
      const payload = { id: this.group.id, ...this.groupForm.getRawValue() };
      this.group_service
        .updateGroup(payload)
        .subscribe({
          next: () => {
            console.log('Group updated successfully')
            this.snackBar.open('group updated','Hide',{duration: 3000});
          },
          error: (error) => console.log(error)
        });
      return;
    }

    this.group_service
      .createGroup(this.groupForm.value)
      .subscribe(
        {
          next: (response) => {
            console.log('Group created successfully')
            console.log(response)
            this.groupCreatedId.emit(response.id)
          },
          error: (error) => {
            console.log(error)
          }
        }
      )
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
}
