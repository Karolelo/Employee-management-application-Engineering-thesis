import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {GradeRoutingModule} from './grade-routing-module';
import { UserGradeListComponent } from './components/user-grade-list/user-grade-list.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';



@NgModule({
  declarations: [
    UserGradeListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    GradeRoutingModule,
    MatProgressSpinnerModule,
  ]
})
export class GradeModule { }
