import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskDetailsComponent } from './components/task-details/task-details.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskFormComponent } from './components/task-form/task-form.component';



@NgModule({
  declarations: [
    TaskDetailsComponent,
    TaskListComponent,
    TaskFormComponent
  ],
  imports: [
    CommonModule
  ]
})
export class TaskModule { }
