import {RouterModule, Routes} from '@angular/router';
import {NgModule} from '@angular/core';
import {UserListComponent} from './components/user-list/user-list.component';
import {AdminDashboardComponent} from './components/admin-dashboard/admin-dashboard.component';

const routes: Routes = [
  {
    path: '', component: AdminDashboardComponent
  },
  {
    path: 'users', component: UserListComponent
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
