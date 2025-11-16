import {RouterModule, Routes,CanDeactivate} from '@angular/router';
import {NgModule,inject} from '@angular/core';
import {UserListComponent} from './components/user-list/user-list.component';
import {AdminDashboardComponent} from './components/admin-dashboard/admin-dashboard.component';
import {CreateUserStepperFormComponent} from './components/create-user-stepper-form/create-user-stepper-form.component';
import {EditUserFormComponent} from './components/edit-user-form/edit-user-form.component';
import {GroupViewPageComponent} from './pages/group-view-page/group-view-page.component';
import {GroupResolverService} from './resolvers/group-resolver.service';
import {GroupsPageComponent} from './pages/groups-page/groups-page.component';
import {ManageGroupPageComponent} from './pages/manage-group-page/manage-group-page.component';
import {GroupFormComponent} from './components/group-form/group-form.component';
import {GroupCreatePageComponent} from './pages/group-create-page/group-create-page.component';
import {GroupEditPageComponent} from './pages/group-edit-page/group-edit-page.component';
import {GroupEditResolverService} from './resolvers/group-edit-resolver.service';


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
    path: `groups`, component: GroupsPageComponent,
    resolve: {groups: GroupResolverService}
  },
  {
    path: 'groups/:id', component: GroupViewPageComponent,
  },
  {
    path: 'groupsManage/:id', component: ManageGroupPageComponent
  },
  {
    path: 'group/create', component: GroupCreatePageComponent,
    canDeactivate: [()=> inject(GroupCreatePageComponent).canDeactivate()]
  },
  {
    path:'group/edit/:id', component: GroupEditPageComponent,
    resolve: {group: GroupEditResolverService}
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
