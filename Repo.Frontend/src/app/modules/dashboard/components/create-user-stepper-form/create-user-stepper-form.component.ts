import {Component,ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatStepperModule,MatStepper} from '@angular/material/stepper';
import {Router} from '@angular/router';
import {DashboardModule} from '../../dashboard.module';
import {User} from '../../interfaces/user';
import {EditUserFormComponent} from '../edit-user-form/edit-user-form.component';
import {MatSnackBar} from '@angular/material/snack-bar'
import {UserService} from '../../services/user/user.service';
@Component({
  selector: 'app-create-user-stepper-form',
  standalone: false,
  templateUrl: './create-user-stepper-form.component.html',
  styleUrl: './create-user-stepper-form.component.css',
})
export class CreateUserStepperFormComponent {
    userRegistrationForm: FormGroup;
    Roles = ['Admin','User','Accountant','TeamLeader'];
    constructor(private fb: FormBuilder,private userService: UserService,private router: Router,
                private snackBar: MatSnackBar) {
      this.userRegistrationForm = this.fb.group({
        Name: ['', Validators.required],
        Surname: ['', Validators.required],
        Nickname: ['', Validators.required],
        Login: ['', [Validators.required,Validators.pattern('^[A-Z]{1}[A-Za-z0-9_]{5,14}$')]],
        Email: ['', [Validators.required,Validators.email]],
        Password: ['', [Validators.required,Validators.minLength(6)]],
        ConfirmPassword: ['', Validators.required],
        Role: [[], Validators.required],
      },
        {
          validator: DashboardModule.passwordMatchValidator
        })
    }

  onSubmit() {

    this.userService.creteUser(this.userRegistrationForm.value)
      .subscribe(
        {
          next: (response) => {
            console.log('User created successfully')
            console.log(response)
            this.router.navigate(['/dashboard/users'])
          },
          error: (error) => {
            console.log(error)
            this.snackBar.open('Error during createing a user: '+error.error.message, 'OK',
              {
                 duration: 9000
                ,panelClass: ['error-snackbar']
              });
          }
        })
  }
}
