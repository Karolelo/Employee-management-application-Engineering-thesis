import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import {AuthService} from './services/auth_managment/auth.service';
import {LoginRequest} from './interafaces/login-request';
import {MatSnackBar} from '@angular/material/snack-bar';
import {OnInit} from '@angular/core';

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

  ngOnInit() {
    this.authService.isAuthenticated().subscribe(isAuth => {
      if (isAuth) {
        this.router.navigate(['/tasks']);
      }
    });
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
        console.error('Error durring login:', error);
        this.snackBar.open('Error during login: ' + error.error.message, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        })
      }
    });
  }
}
