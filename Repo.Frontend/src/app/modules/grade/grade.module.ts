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



@NgModule({
  declarations: [
    UserGradeListComponent,
    GradePageComponent,
    GradeFormComponent
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
