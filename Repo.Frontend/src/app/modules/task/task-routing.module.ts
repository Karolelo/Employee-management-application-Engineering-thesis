import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TaskPageComponent } from './pages/task-page/task-page.component';
import {TaskDetailsComponent} from './components/task-details/task-details.component';
import {TasksDetailPageComponent} from './pages/tasks-detail-page/tasks-detail-page.component';

const routes: Routes = [
  {
    path: '',
    component: TaskPageComponent
  },
  {
    path: 'task-details/:id',
    component: TasksDetailPageComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TaskRoutingModule { }
