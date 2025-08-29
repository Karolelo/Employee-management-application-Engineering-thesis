import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskDetailsComponent } from './components/task-details/task-details.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskFormComponent } from './components/task-form/task-form.component';
import {ReactiveFormsModule} from "@angular/forms";
import { TaskPageComponent } from './pages/task-page/task-page.component';
import {TaskRoutingModule} from './task-routing.module';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {NgClass} from '@angular/common';
import {MatProgressBar} from "@angular/material/progress-bar";


@NgModule({
  declarations: [
    TaskDetailsComponent,
    TaskListComponent,
    TaskFormComponent,
    TaskPageComponent
  ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        TaskRoutingModule,
        MatProgressSpinner,
        MatProgressBar
    ]
})
export class TaskModule { }
