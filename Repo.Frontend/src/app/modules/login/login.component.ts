import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import {AuthService} from './services/auth.service';
import {LoginRequest} from './interafaces/login-request';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private router = inject(Router);
  nickname: string ='';
  password: string='';

  constructor(private authService: AuthService, private snackBar: MatSnackBar) {
  }
  LogIn() {
    const credentials: LoginRequest = {
      nickname: this.nickname,
      password: this.password
    };

    this.authService.login(credentials).subscribe({
      next: () => {
        this.router.navigate(['']);
      },
      error: (error) => {
        console.error('Błąd logowania:', error);
        this.snackBar.open('Error during login: ' + error.message, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        })
      }
    });
  }
}
