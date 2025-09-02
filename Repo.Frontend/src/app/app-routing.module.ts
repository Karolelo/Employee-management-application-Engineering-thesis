import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './modules/login/login.component';
import {AuthLayoutComponent} from './layouts/auth-layout/auth-layout.component';
import {MainLayoutComponent} from './layouts/main-layout/main-layout.component';
import {CalendarModule} from './modules/calendar-module/calendar.module';
import {TaskPageComponent} from './modules/task/pages/task-page/task-page.component';
import {TaskModule} from './modules/task/task.module';
import {AuthGuardService} from './guard/AuthGuard/auth-guard.service';
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
