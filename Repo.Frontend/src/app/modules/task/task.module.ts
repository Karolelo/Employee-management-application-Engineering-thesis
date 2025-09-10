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
import { TasksDetailPageComponent } from './pages/tasks-detail-page/tasks-detail-page.component';
import {
  MatCell, MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow,
  MatRowDef,
  MatTable
} from "@angular/material/table";
import {MatPaginator, MatPaginatorModule} from '@angular/material/paginator';


@NgModule({
  declarations: [
    TaskDetailsComponent,
    TaskListComponent,
    TaskFormComponent,
    TaskPageComponent,
    TasksDetailPageComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TaskRoutingModule,
    MatProgressSpinner,
    MatProgressBar,
    NgIf,
    NgClass,
    MatSlideToggle,
    MatSelect,
    MatLabel,
    MatOption,
    FormsModule,
    MatTable,
    MatColumnDef,
    MatHeaderCell,
    MatCell,
    MatHeaderRow,
    MatRow,
    MatRowDef,
    MatHeaderRowDef,
    MatHeaderCellDef,
    MatCellDef,
    MatPaginator,
  ]
})
export class TaskModule { }
