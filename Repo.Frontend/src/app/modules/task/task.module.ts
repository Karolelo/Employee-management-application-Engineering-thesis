import { NgModule } from '@angular/core';
import {CommonModule, NgClass, NgIf} from '@angular/common';
import { TaskDetailsComponent } from './components/task-details/task-details.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskFormComponent } from './components/task-form/task-form.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { TaskPageComponent } from './pages/task-page/task-page.component';
import {TaskRoutingModule} from './task-routing.module';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatProgressBar} from "@angular/material/progress-bar";
import {MatSlideToggle} from '@angular/material/slide-toggle';
import {MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';


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
    MatProgressBar,
    NgIf,
    NgClass,
    NgClass,
    NgClass,
    MatSlideToggle,
    MatSelect,
    MatLabel,
    MatOption,
    FormsModule
  ]
})
export class TaskModule { }
