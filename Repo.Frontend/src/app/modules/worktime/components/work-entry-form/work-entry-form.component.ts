import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WorkEntry } from '../../interfaces/work-entry';
import { WorkEntryService } from '../../services/work-entry.service';
import { UserMini } from '../../interfaces/user-mini';
import { WorktimeUserService } from '../../services/worktime-user.service';

@Component({
  selector: 'app-work-entry-form',
  standalone: false,
  templateUrl: './work-entry-form.component.html',
  styleUrls: ['./work-entry-form.component.css']
})
export class WorkEntryFormComponent implements OnInit, OnChanges {
  @Input() entryToEdit?: WorkEntry;
  @Output() entrySaved = new EventEmitter<void>();

  workEntryForm!: FormGroup;
  users: UserMini[] = [];
  loading = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private workEntryService: WorkEntryService,
    private worktimeUserService: WorktimeUserService
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entryToEdit']) {
      if (this.entryToEdit) {
        this.patchForm(this.entryToEdit);
        this.workEntryForm.get('user_ID')?.disable();
      } else {
        this.workEntryForm.get('user_ID')?.enable();
        this.resetForm();
      }
    }
  }

  private initializeForm(): void {
    this.workEntryForm = this.fb.group({
      user_ID: [null, Validators.required],
      work_Date: ['', Validators.required],   // 'YYYY-MM-DD'
      hours_Worked: [1, [Validators.required, Validators.min(0.25)]],
      task_ID: [null],
      comment: ['']
    });
  }

  private loadUsers(): void {
    this.worktimeUserService.getUsers().subscribe({
      next: users => (this.users = users),
      error: err => {
        console.error('Error loading users', err);
        this.errorMessage = 'Error loading users';
      }
    });
  }

  private patchForm(entry: WorkEntry): void {
    this.workEntryForm.patchValue({
      work_Date: entry.work_Date,
      hours_Worked: entry.hours_Worked,
      task_ID: entry.task_ID ?? null,
      comment: entry.comment ?? ''
    });
  }

  onSubmit(): void {
    if (this.workEntryForm.invalid) {
      this.workEntryForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = null;

    const raw = this.workEntryForm.getRawValue();
    const payload = {
      work_Date: raw.work_Date,
      hours_Worked: raw.hours_Worked,
      task_ID: raw.task_ID,
      comment: raw.comment
    };

    if (this.entryToEdit) {
      this.workEntryService.updateEntry(this.entryToEdit.id, payload).subscribe({
        next: () => {
          this.loading = false;
          this.resetForm();
          this.entrySaved.emit();
        },
        error: err => {
          this.loading = false;
          this.errorMessage = err.error?.message || 'Error updating work entry';
          console.error(err);
        }
      });
    } else {
      const userId = raw.user_ID;
      if (!userId) {
        this.loading = false;
        this.errorMessage = 'User is required.';
        return;
      }

      this.workEntryService.createEntryForUser(userId, payload).subscribe({
        next: () => {
          this.loading = false;
          this.resetForm();
          this.entrySaved.emit();
        },
        error: err => {
          this.loading = false;
          this.errorMessage = err.error?.message || 'Error creating work entry';
          console.error(err);
        }
      });
    }
  }

  resetForm(): void {
    this.workEntryForm.reset({
      user_ID: null,
      work_Date: '',
      hours_Worked: 1,
      task_ID: null,
      comment: ''
    });
    this.entryToEdit = undefined;
    this.workEntryForm.get('user_ID')?.enable();
  }
}
