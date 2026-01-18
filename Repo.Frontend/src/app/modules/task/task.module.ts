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
import {MatPaginator} from '@angular/material/paginator';
import { GroupTaskFormComponent } from './components/group-task-form/group-task-form.component';
import { GroupTaskListComponent } from './components/group-task-list/group-task-list.component';
import {InfoMessageComponent} from "../../common_components/info-message/info-message.component";
import { TaskGanttPageComponent } from './pages/task-gantt-page/task-gantt-page.component';
import { TaskGanttComponent } from './components/task-gantt/task-gantt.component';
import {UserSwitchOverlayComponent} from './components/user-switch-overlay/user-switch-overlay.component';

import {MatFormField} from '@angular/material/form-field'

@NgModule({
    declarations: [
        TaskDetailsComponent,
        TaskListComponent,
        TaskFormComponent,
        TaskPageComponent,
        TasksDetailPageComponent,
        GroupTaskFormComponent,
        GroupTaskListComponent,
        TaskGanttPageComponent,
        TaskGanttComponent,
        UserSwitchOverlayComponent
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
        InfoMessageComponent,
        MatFormField
    ],
  exports: [
    TaskFormComponent,
    TaskListComponent,
    TaskPageComponent,
    GroupTaskFormComponent,
    GroupTaskListComponent
  ],
    providers: []
})
export class TaskModule { }
