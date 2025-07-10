import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './login/login.component';
import {AuthLayoutComponent} from './layouts/auth-layout/auth-layout.component';
import {MainLayoutComponent} from './layouts/main-layout/main-layout.component';
import {TasksComponent} from './task_module/tasks/tasks.component';
import {TaskCreatorComponent} from './task_module/task-creator/task-creator.component';
import {TaskDetailsComponent} from './task_module/components/task-details/task-details.component';
import {CalendarModule} from './calendar-module/calendar.module';
const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: 'tasks',component: TasksComponent},
      { path: 'tasks/create', component: TaskCreatorComponent},
      { path: 'details-task/:id', component: TaskDetailsComponent},
      { path: 'calendar', loadChildren: () => CalendarModule,}
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
