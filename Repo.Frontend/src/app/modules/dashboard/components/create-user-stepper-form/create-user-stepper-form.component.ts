import {Component,ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatStepperModule,MatStepper} from '@angular/material/stepper';
@Component({
  selector: 'app-create-user-stepper-form',
  standalone: false,
  templateUrl: './create-user-stepper-form.component.html',
  styleUrl: './create-user-stepper-form.component.css',
})
export class CreateUserStepperFormComponent {
    userRegistrationForm: FormGroup;
    /*@ViewChild('stepper') stepper!: MatStepper;*/
    constructor(private fb: FormBuilder) {
      this.userRegistrationForm = this.fb.group({
        Name: ['', Validators.required],
        Surname: ['', Validators.required],
        Nickname: ['', Validators.required],
        Login: ['', [Validators.required,Validators.pattern('^[A-Z]{1}[A-Za-z0-9]{5,14}$')]],
        Email: ['', Validators.required,Validators.email],
        Password: ['', [Validators.required,Validators.minLength(6)]],
        ConfirmPassword: ['', Validators.required],
        Role: [[], Validators.required],
      },
        {
          validator: this.passwordMatchValidator
        })
    }

    passwordMatchValidator(form: FormGroup) {
      const password = this.userRegistrationForm.get('Password')?.value;
      const confirmed = this.userRegistrationForm.get('ConfirmPassword')?.value;
      return password === confirmed ? null : {passwordMismatch: true};
    }

  //Only creating this method because of bug of my IDE which not finding in HTML file
  //Imported components from material, on some computeres it finds, dont know why
  /*onReset() {
    this.stepper.reset()
  }*/
}
