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



@NgModule({
  declarations: [
    AdminDashboardComponent,
    UserListComponent
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
    MatFormField
  ]
})
export class DashboardModule { }
