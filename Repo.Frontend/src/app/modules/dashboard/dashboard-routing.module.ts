import {RouterModule, Routes} from '@angular/router';
import {NgModule} from '@angular/core';
import {UserListComponent} from './components/user-list/user-list.component';
import {AdminDashboardComponent} from './components/admin-dashboard/admin-dashboard.component';
import {CreateUserStepperFormComponent} from './components/create-user-stepper-form/create-user-stepper-form.component';
import {EditUserFormComponent} from './components/edit-user-form/edit-user-form.component';


const routes: Routes = [
  {
    path: '', component: AdminDashboardComponent
  },
  {
    path: 'users', component: UserListComponent,
  },
  {
    path: 'users/create', component: CreateUserStepperFormComponent
  },
  {
    path: 'users/edit/:id', component: EditUserFormComponent
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
