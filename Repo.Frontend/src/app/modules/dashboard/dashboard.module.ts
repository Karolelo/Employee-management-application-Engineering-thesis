import { NgModule } from '@angular/core';
import { CommonModule,NgOptimizedImage } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { MatGridListModule} from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { UserListComponent } from './components/user-list/user-list.component';
import { DashboardRoutingModule} from './dashboard-routing.module';
import { MatToolbar} from '@angular/material/toolbar';
import { MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import { DeleteUserDialogComponent } from './components/delete-user-dialog/delete-user-dialog.component';
import { CreateUserStepperFormComponent } from './components/create-user-stepper-form/create-user-stepper-form.component';
import { MatStepperModule } from '@angular/material/stepper';
import { ReactiveFormsModule,FormGroup } from '@angular/forms';
import { MatSelectModule} from '@angular/material/select';
import { EditUserFormComponent } from './components/edit-user-form/edit-user-form.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { CdkStepperModule } from '@angular/cdk/stepper';
import { GroupsPageComponent } from './pages/groups-page/groups-page.component';
import { ManageGroupPageComponent } from './pages/manage-group-page/manage-group-page.component';
import { GroupFormComponent } from './components/group-form/group-form.component';
import { GroupListComponent } from './components/group-list/group-list.component';
import { TaskModule} from "../task/task.module";
import { AnnouncementFormComponent } from './components/annoucment-form/announcement-form.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { AnnouncementListComponent } from './components/announcement-list/announcement-list.component';
import { TextFieldModule } from '@angular/cdk/text-field';
import { MatDividerModule,MatDivider} from '@angular/material/divider';
import { GroupCreatePageComponent } from './pages/group-create-page/group-create-page.component';
import { AddUserToGroupFormComponent } from './components/add-user-to-group-form/add-user-to-group-form.component';
import { MatProgressBar} from '@angular/material/progress-bar';
import { MatSpinner} from '@angular/material/progress-spinner';
import { GroupShortInfoComponent } from './components/group-short-info/group-short-info.component';
import { GroupImageEditFormComponent} from './components/group-image-edit-form/group-image-edit-form.component';
import {InfoMessageComponent} from '../../common_components/info-message/info-message.component';
import { UserSimpleListComponent } from './components/user-simple-list/user-simple-list.component';
import {ExportImageComponent} from '../../common_components/export-image/export-image.component';
import { GroupEditPageComponent } from './pages/group-edit-page/group-edit-page.component';
import { TaskStatsComponent } from './components/task-stats/task-stats.component';
import { BaseChartDirective } from 'ng2-charts';
@NgModule({
  declarations: [
    AdminDashboardComponent,
    UserListComponent,
    DeleteUserDialogComponent,
    CreateUserStepperFormComponent,
    EditUserFormComponent,
    GroupsPageComponent,
    ManageGroupPageComponent,
    GroupFormComponent,
    GroupListComponent,
    AnnouncementFormComponent,
    AnnouncementListComponent,
    GroupCreatePageComponent,
    AddUserToGroupFormComponent,
    GroupShortInfoComponent,
    GroupImageEditFormComponent,
    UserSimpleListComponent,
    GroupEditPageComponent,
    TaskStatsComponent,
  ],
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatGridListModule,
    MatCardModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    DashboardRoutingModule,
    MatToolbar,
    MatLabel,
    MatInput,
    MatSuffix,
    MatFormField,
    MatStepperModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatSnackBarModule,
    CdkStepperModule,
    TaskModule,
    NgOptimizedImage,
    MatFormFieldModule,
    TextFieldModule,
    MatDividerModule,
    MatProgressBar,
    MatSpinner,
    InfoMessageComponent,
    MatDivider,
    ExportImageComponent,
    BaseChartDirective
  ],
  exports: [
    UserSimpleListComponent,
    AnnouncementListComponent,
    GroupShortInfoComponent,
    TaskStatsComponent
  ],
  providers: [
    GroupCreatePageComponent
  ]
})
export class DashboardModule {

  public static passwordMatchValidator(form: FormGroup) {
    const password = form.get('Password')?.value;
    const confirmed = form.get('ConfirmPassword')?.value;
    return password === confirmed ? null : {passwordMismatch: true};
  }

}
