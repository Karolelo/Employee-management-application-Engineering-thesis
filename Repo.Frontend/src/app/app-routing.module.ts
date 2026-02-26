import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './modules/login/login.component';
import {AuthLayoutComponent} from './layouts/auth-layout/auth-layout.component';
import {MainLayoutComponent} from './layouts/main-layout/main-layout.component';
import {CalendarModule} from './modules/calendar-module/calendar.module';
import {TaskModule} from './modules/task/task.module';
import {GradeModule} from './modules/grade/grade.module';
import {WorktimeModule} from './modules/worktime/worktime.module';
import {AuthGuardService} from './guard/authGuard/auth-guard.service';
import {DashboardModule} from './modules/dashboard/dashboard.module';
import {RoleGuardService} from './guard/roleGuard/role-guard.service';
import {ForbiddenPage403Component} from './common_components/forbidden-page403/forbidden-page403.component';
import {NotFoundPage404Component} from './common_components/not-found-page404/not-found-page404.component';
import {ProfileModule} from './modules/profile/profile.module';
import {GroupModule} from './modules/group/group.module';

const routes: Routes = [
  {
    path: 'login',
    component: AuthLayoutComponent,
    children: [
      { path: '', component: LoginComponent }
    ]
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuardService],
    canActivateChild: [RoleGuardService],
    children: [
      {
        path: 'tasks', loadChildren: () => TaskModule,
        data: { title: 'Tasks module', expectedRoles:['User','Admin','TeamLeader','Accountant']}
      },
      {
        path: 'calendar', loadChildren: () => CalendarModule,
        data: { title: 'Calendar module', expectedRoles:['User','Admin','TeamLeader','Accountant']}
      },
      {
        path: 'dashboard', loadChildren: () => DashboardModule,
        data: { title: 'Dashboard module', expectedRoles: ['Admin','TeamLeader']}
      },
      {
        path: 'grades', loadChildren: () => GradeModule,
        data: { title: 'Grades module', expectedRoles: ['User','Admin','TeamLeader']}
      },
      {
        path: 'worktime', loadChildren: () => WorktimeModule,
        data: { title: 'Worktime module', expectedRoles: ['User','Admin','Accountant']}
      },
      {
        path: 'profile', loadChildren: () => ProfileModule,
        data: { title: 'Profile module', expectedRoles: ['User','Admin','Accountant']}
      },
      {
        path: 'forbidden', component: ForbiddenPage403Component,
        data: { title: '', expectedRoles: ['User','Admin','TeamLeader','Accountant']},
        canActivateChild:[]
      },
      {
        path: '404', component: NotFoundPage404Component,
        data: { title: '', expectedRoles: ['User','Admin','TeamLeader','Accountant'],},
        canActivateChild: []
      },
      {
        path: 'myGroups', loadChildren: () => GroupModule,
        data: {title: 'My groups', expectedRoles: ['User','Admin','TeamLeader','Accountant']}
      }
    ]
  },
    { path: '**', redirectTo: '/404' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
