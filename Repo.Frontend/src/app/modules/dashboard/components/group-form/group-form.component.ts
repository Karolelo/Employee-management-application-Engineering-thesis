import { Component,Input } from '@angular/core';
import { Form,FormBuilder,FormGroup,Validators } from '@angular/forms';
import {GroupService} from '../../services/group/group.service';
import {UserService} from '../../services/user/user.service';
import {User} from '../../interfaces/user';
import {Router} from '@angular/router';
@Component({
  selector: 'app-group-form',
  standalone: false,
  templateUrl: './group-form.component.html',
  styleUrl: './group-form.component.css'
})
export class GroupFormComponent {
  groupForm: FormGroup;
  teamLeaders: User[] = [];
  constructor(private fb: FormBuilder
              ,private user_service: UserService
              ,private group_service: GroupService
              ,private router: Router) {
    this.groupForm = this.fb.group(
      {
        name: ['', Validators.required],
        description: ['', [Validators.required,Validators.minLength(30),Validators.maxLength(1000)]],
        admin_ID: ['', Validators.pattern(/^\d+$/)]
      }
    )

    user_service.getUsersWithRole("TeamLeader").subscribe(
      {
        next: (users: User[]) => {
          this.teamLeaders = users;
        },
        error: (error) => console.log(error)
      }
    )
  }

  onSubmit() {
    this.group_service
      .createGroup(this.groupForm.value)
      .subscribe(
        {
          next: (response) => {
            console.log('Group created successfully')
            console.log(response)
            this.router.navigate(['/dashboard'])
          },
          error: (error) => {
            console.log(error)
          }
        }
      )
  }
}
