import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
// import { ProfileLandingPageComponent } from '../pages/profile-landing-page/profile-landing-page.component';
import { ProfileLandingPageComponent } from './pages/profile-landing-page/profile-landing-page.component';

const routes: Routes = [
  {
    path: '',
    component: ProfileLandingPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
