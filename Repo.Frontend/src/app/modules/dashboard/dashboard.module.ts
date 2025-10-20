import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
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
import {DashboardRoutingModule} from './dashboard-routing.module';
import {MatToolbar} from '@angular/material/toolbar';
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import { DeleteUserDialogComponent } from './components/delete-user-dialog/delete-user-dialog.component';
import { CreateUserStepperFormComponent } from './components/create-user-stepper-form/create-user-stepper-form.component';
import { MatStepperModule } from '@angular/material/stepper';
import { ReactiveFormsModule,FormGroup } from '@angular/forms';
import {MatSelectModule} from '@angular/material/select';
import { EditUserFormComponent } from './components/edit-user-form/edit-user-form.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { CdkStepperModule } from '@angular/cdk/stepper';
import { GroupsPageComponent } from './pages/groups-page/groups-page.component';
import { ManageGroupPageComponent } from './pages/manage-group-page/manage-group-page.component';
import { GroupViewPageComponent } from './pages/group-view-page/group-view-page.component';
import { GroupFormComponent } from './components/group-form/group-form.component';
@NgModule({
  declarations: [
    AdminDashboardComponent,
    UserListComponent,
    DeleteUserDialogComponent,
    CreateUserStepperFormComponent,
    EditUserFormComponent,
    GroupsPageComponent,
    ManageGroupPageComponent,
    GroupViewPageComponent,
    GroupFormComponent,
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
    CdkStepperModule
  ]
})
export class DashboardModule {

  //TODO wynieść może do share module
  public static passwordMatchValidator(form: FormGroup) {
    const password = form.get('Password')?.value;
    const confirmed = form.get('ConfirmPassword')?.value;
    return password === confirmed ? null : {passwordMismatch: true};
  }

}
