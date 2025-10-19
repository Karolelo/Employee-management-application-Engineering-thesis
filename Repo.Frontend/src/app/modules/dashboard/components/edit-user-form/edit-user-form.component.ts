import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DashboardModule} from '../../dashboard.module';
import {UserService} from '../../services/user.service';
import {ActivatedRoute,Router} from '@angular/router';
import {User} from '../../interfaces/user';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
@Component({
  selector: 'app-edit-user-form',
  standalone: false,
  templateUrl: './edit-user-form.component.html',
  styleUrl: './edit-user-form.component.css'
})
export class EditUserFormComponent implements OnInit{

  userEditForm!: FormGroup;
  Roles = ['Admin','User','Accountant','TeamLeader'];
  private userId!: number;
  constructor(private fb: FormBuilder
              , private userService: UserService
              , private route: ActivatedRoute
              , private router: Router ) {
    this.initForm();
  }

  private initForm(): void {
    this.userEditForm = this.fb.group({
        Name: ['', Validators.required],
        Surname: ['', Validators.required],
        Nickname: ['', Validators.required],
        Login: ['', [Validators.required,Validators.pattern('^[A-Z]{1}[A-Za-z0-9_]{5,14}$')]],
        Email: ['', [Validators.required,Validators.email]],
        Password: ['', [Validators.required,Validators.minLength(6)]],
        ConfirmPassword: ['', Validators.required],
        Roles: [[], Validators.required],
      },
      {
        PasswordMismatch: DashboardModule.passwordMatchValidator
      });
  }

  ngOnInit() {
    this.route.params.subscribe({
      next: (params) => {
        this.userId = params['id'];
        console.log("User ID: " + this.userId);
        if (this.userId) {
          this.userService.getUser(this.userId).subscribe({
            next: (data) => {
              this.updateFormWithUserData(data)
            },
            error: (error) => console.log("Error during fetching user data: " + error)
          });
        } else {
          console.log("Error: User ID is null");
        }
      },
      error: (error) => {
        console.log("Error during initializing component: " + error);
      }
    });
  }

  private updateFormWithUserData(user: User): void {
    console.log("User data: ",user);
    if (!user) return;

    this.userEditForm.patchValue({
      Name: user.name,
      Surname: user.surname,
      Nickname: user.nickname,
      Login: user.login,
      Email: user.email,
      Password: user.password,
      ConfirmPassword: user.password,
      Roles: user.roles
    });
  }

  onSubmit() {
    const formData = {
      ...this.userEditForm.value,
      ID: this.userId
    };
    console.log("Form data: ", formData);
    this.userService
      .updateUser(formData)
      .subscribe({
        next: () => {
          console.log("User updated successfully");
          this.router.navigate(['/dashboard/users']);
        },
        error: (error) => {
          console.log("Error during user update: ", error);
        }
      });
  }
}
