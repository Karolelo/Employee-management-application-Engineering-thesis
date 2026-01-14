import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TaskPageComponent } from './pages/task-page/task-page.component';
import {TasksDetailPageComponent} from './pages/tasks-detail-page/tasks-detail-page.component';
import {taskGuard} from './guards/task.guard';
import {TaskGanttPageComponent} from './pages/task-gantt-page/task-gantt-page.component';

const routes: Routes = [
  {
    path: '',
    component: TaskPageComponent
  },
  {
    path: 'task-details/:id',
    component: TasksDetailPageComponent,
    canActivate: [taskGuard]
  },
  { path: 'gantt',
    component: TaskGanttPageComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TaskRoutingModule { }
