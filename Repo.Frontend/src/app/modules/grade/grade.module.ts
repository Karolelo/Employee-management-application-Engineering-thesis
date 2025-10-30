import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {GradeRoutingModule} from './grade-routing-module';
import { UserGradeListComponent } from './components/user-grade-list/user-grade-list.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { GradePageComponent } from './pages/grade-page/grade-page.component';
import { GradeFormComponent } from './components/grade-form/grade-form.component';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import { GradeDetailsComponent } from './components/grade-details/grade-details.component';
import { GradeDetailPageComponent } from './pages/grade-detail-page/grade-detail-page.component';
import { TargetListComponent } from './components/target-list/target-list.component';
import { CourseListComponent } from './components/course-list/course-list.component';
import { CourseEnrollOverlayComponent } from './components/course-enroll-overlay/course-enroll-overlay.component';
import { TargetDetailsOverlayComponent } from './components/target-details-overlay/target-details-overlay.component';
import { GradeDetailsOverlayComponent } from './components/grade-details-overlay/grade-details-overlay.component';



@NgModule({
  declarations: [
    UserGradeListComponent,
    GradePageComponent,
    GradeFormComponent,
    GradeDetailsComponent,
    GradeDetailPageComponent,
    TargetListComponent,
    CourseListComponent,
    CourseEnrollOverlayComponent,
    TargetDetailsOverlayComponent,
    GradeDetailsOverlayComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    GradeRoutingModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ]
})
export class GradeModule { }
