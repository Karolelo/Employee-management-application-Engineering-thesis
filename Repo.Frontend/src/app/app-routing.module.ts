import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './modules/login/login.component';
import {AuthLayoutComponent} from './layouts/auth-layout/auth-layout.component';
import {MainLayoutComponent} from './layouts/main-layout/main-layout.component';
import {CalendarModule} from './modules/calendar-module/calendar.module';
import {TaskModule} from './modules/task/task.module';
import {AuthGuardService} from './guard/AuthGuard/auth-guard.service';
import {DashboardModule} from './modules/dashboard/dashboard.module';
const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuardService],
    children: [
      { path: 'tasks', loadChildren: () => TaskModule,
      data: { title: 'Tasks module',expectedRole:'User'}},
      { path: 'calendar', loadChildren: () => CalendarModule,
      data: { title: 'Calendar module',expectedRole:'User' }},
      //TODO zmienić expected role na admin i wyświetlać żę lipa kolego bez admina albo
      //Team leada nie da się zalogować
      { path: 'dashboard', loadChildren: () => DashboardModule,
      data: { title: 'Dashboard module',expectedRole:'User'}}
    ]
  },
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent }
    ]
  }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
