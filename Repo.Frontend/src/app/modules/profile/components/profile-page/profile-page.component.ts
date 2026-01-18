import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile.service';
import { UserProfile } from '../../interfaces/user-profile';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.css',
  standalone: false,
})
export class ProfilePageComponent implements OnInit {
  profile: UserProfile | null = null;

  emailForm = {
    currentPassword: '',
    newEmail: '',
  };

  passwordForm = {
    currentPassword: '',
    newPassword: '',
  };

  constructor(private profileService: ProfileService, private snackBar: MatSnackBar) {}

  ngOnInit(): void {
    this.profileService.getProfile().subscribe({
      next: p => (this.profile = p),
      error: err => {
        console.error(err);
        this.snackBar.open('Failed to load profile', 'Close', { duration: 3000 });
      },
    });
  }

  onChangeEmail(): void {
    this.profileService.changeEmail(this.emailForm).subscribe({
      next: () => {
        this.snackBar.open('Email updated', 'Close', { duration: 3000 });
        if (this.profile) this.profile.email = this.emailForm.newEmail;
        this.emailForm.currentPassword = '';
      },
      error: err => {
        this.snackBar.open(err.error?.message || 'Email update failed', 'Close', { duration: 4000 });
      },
    });
  }

  onChangePassword(): void {
    this.profileService.changePassword(this.passwordForm).subscribe({
      next: () => {
        this.snackBar.open('Password updated', 'Close', { duration: 3000 });
        this.passwordForm.currentPassword = '';
        this.passwordForm.newPassword = '';
      },
      error: err => {
        this.snackBar.open(err.error?.message || 'Password update failed', 'Close', { duration: 4000 });
      },
    });
  }
}
