import { Component, Input } from '@angular/core';
import { FormGroup, FormControl, Validators,FormBuilder } from '@angular/forms';
import {Announcement} from '../../interfaces/announcement';
import { MatFormFieldModule,MatFormFieldAppearance } from '@angular/material/form-field';
@Component({
  selector: 'app-announcement-form',
  standalone: false,
  template: `
    <h1 class="title-header">{{announcement ? "Create announcement" : "Edit announcement"}}</h1>
    <form [formGroup]="form" class="form-container" (submit)="onSubmit()">
        <mat-form-field appearance="outline">
          <mat-label>Title</mat-label>
          <input matInput formControlName="title" placeholder="Title">
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Text</mat-label>
          <input matInput formControlName="text" placeholder="Title">
        </mat-form-field>
        <button mat-raised-button type="submit">Submit</button>
    </form>
  `,
  styleUrl: './announcement-form.component.css'
})
export class AnnouncementFormComponent {
  @Input() announcement?: Announcement;
  form: FormGroup;
  constructor(private fb: FormBuilder) {
    this.form = fb.group(
      {
        title: ['', [Validators.required,Validators.minLength(5)]],
        text: ['', Validators.required, Validators.minLength(12)],
        group_ID: ['', Validators.pattern('\d{1,}')]
      })

      if(this.announcement)
        this.initValues(
          this.announcement
        )
  }

  initValues(data: Announcement) {
    this.form.patchValue(data);
  }

  onSubmit() {

  }
}
