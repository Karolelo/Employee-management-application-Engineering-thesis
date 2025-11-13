import { NgModule } from '@angular/core';
import { CommonModule, NgIf, NgForOf, NgClass } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { WorktimeRoutingModule } from './worktime-routing.module';

import { WorkEntryFormComponent } from './components/work-entry-form/work-entry-form.component';

import { MatProgressSpinner } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [
    WorkEntryFormComponent,
  ],
  imports: [
    CommonModule,
    WorktimeRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    NgIf,
    NgForOf,
    NgClass,
    MatProgressSpinner
  ]
})
export class WorktimeModule { }
