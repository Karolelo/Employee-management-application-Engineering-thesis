import { Component, Input,OnChanges,SimpleChanges } from '@angular/core';
import { FormGroup, FormControl, Validators,FormBuilder } from '@angular/forms';
import {Announcement} from '../../interfaces/announcement';
import { MatFormFieldModule,MatFormFieldAppearance } from '@angular/material/form-field';
import {AnnouncementService} from '../../services/announcement/announcement.service';
import { TextFieldModule } from '@angular/cdk/text-field';
import {MatDividerModule} from '@angular/material/divider';
import {MatSnackBar} from '@angular/material/snack-bar';
@Component({
  selector: 'app-announcement-form',
  standalone: false,
  template: `
    <h2 class="title-header">{{announcement?"Edit announcement":"Create announcement"}}</h2>
    <form [formGroup]="form" class="form-container" (submit)="onSubmit()">
        <mat-form-field appearance="outline">
          <mat-label>Title</mat-label>
          <input matInput formControlName="title" placeholder="Title">
          <mat-error *ngIf="form.get('title')?.errors?.['required']">
            Title is required.
          </mat-error>
          <mat-error *ngIf="form.get('title')?.errors?.['minlength']">
            Title needs to be at least 5 characters long.
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Text</mat-label>
          <textarea
            matInput
            formControlName="text"
            placeholder="Announcement content"
            rows="4"
            cdkTextareaAutosize
            cdkAutosizeMinRows="4"
            cdkAutosizeMaxRows="8">
          </textarea>
          <mat-error *ngIf="form.get('text')?.errors?.['required']">
            Text is required.
          </mat-error>
          <mat-error *ngIf="form.get('text')?.errors?.['minlength']">
            Text needs to be at least 12 characters long.
          </mat-error>
          <mat-error *ngIf="form.get('text')?.errors?.['maxlength']">
            Text needs to be max 45 characters long.
          </mat-error>
        </mat-form-field>

        <button mat-raised-button type="submit">Submit</button>
    </form>
  `,
  styleUrl: './announcement-form.component.css'
})
export class AnnouncementFormComponent implements OnChanges{
  @Input() announcement?: Announcement;
  @Input() group_ID: number = 0;
  form: FormGroup;
  constructor(private fb: FormBuilder,private annoucement_service:AnnouncementService,private snackBar: MatSnackBar) {
    this.form = fb.group(
      {
        title: ['', [Validators.required,Validators.minLength(5)]],
        text: ['', [Validators.required, Validators.minLength(12),Validators.maxLength(45)]]
      })

      if(this.announcement)
        this.initValues(
          this.announcement
        )
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['announcement'] && changes['announcement'].currentValue) {
      this.initValues(changes['announcement'].currentValue);
    }
  }

  initValues(data: Announcement) {
    this.form.patchValue(data);
  }

  onSubmit() {
    if (!this.announcement && this.annoucement_service.hasReachedAnnouncementLimit(this.group_ID)) {
      this.snackBar.open('Breach limit of announcemt delete \n one to create another', 'Close', {
        duration: 10000,
        panelClass: ['error-snackbar']
      })
      return;
    }
    if (this.form.valid) {
      const formData = this.form.value;

      if (this.announcement) {
        const updateData = {
          ...formData,
          id: this.announcement.id,
          group_ID: this.announcement.group_ID
        };

        this.annoucement_service.updateAnnouncementForGroup(updateData).subscribe({
          next: () => {
            this.resetValues();
            console.log('Announcement updated successfully');
          },
          error: (error) => console.error('Error updating announcement:', error)
        });
      } else {

        const createData = {
          ...formData,
          group_ID: this.group_ID
        };

        this.annoucement_service.createAnnouncementForGroup(createData).subscribe({
          next: () => {
            this.resetValues();
            console.log('Announcement created successfully');
          },
          error: (error) => console.error('Error creating announcement:', error)
        });
      }
    }
  }

  private resetValues() {
    this.announcement = undefined;
    this.form.reset()
  }
}
