import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProfileRoutingModule } from './profile-routing-module';
import { ProfileLandingPageComponent } from './pages/profile-landing-page/profile-landing-page.component';
import { ProfilePageComponent } from './components/profile-page/profile-page.component';

@NgModule({
  declarations: [
    ProfileLandingPageComponent,
    ProfilePageComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    ProfileRoutingModule,
  ],
})
export class ProfileModule {}
