import {RouterModule, Routes} from '@angular/router';
import {NgModule} from '@angular/core';
import {UserListComponent} from './components/user-list/user-list.component';
import {AdminDashboardComponent} from './components/admin-dashboard/admin-dashboard.component';
import {CreateUserStepperFormComponent} from './components/create-user-stepper-form/create-user-stepper-form.component';
import {EditUserFormComponent} from './components/edit-user-form/edit-user-form.component';
import {GroupViewPageComponent} from './pages/group-view-page/group-view-page.component';
import {GroupResolverService} from './resolvers/group-resolver.service';
import {GroupsPageComponent} from './pages/groups-page/groups-page.component';


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
  },
  {
    path: `groups`, component: GroupViewPageComponent,
    resolve: {groups: GroupResolverService}
  },
  {
    path: 'groups/:id', component: GroupsPageComponent,
    data: {

    }
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
