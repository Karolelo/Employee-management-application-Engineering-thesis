import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {Grade} from '../../interfaces/grade';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {GradeService} from '../../services/grade.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-grade-form',
  standalone: false,
  templateUrl: './grade-form.component.html',
  styleUrl: './grade-form.component.css'
})
export class GradeFormComponent implements OnInit, OnChanges{
  @Input() gradeToEdit?: Grade;
  @Output() changed = new EventEmitter<void>();

  form!: FormGroup;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private gradeService: GradeService,
    private userStore: UserStoreService
  ) {
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      grade: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
      description: ['', [Validators.required]],
      start_Date: ['', [Validators.required]],
      finish_Date: ['', [Validators.required]],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['gradeToEdit'] && this.gradeToEdit) {
      this.form.patchValue(this.gradeToEdit);
    }
  }

  save(): void {
    if (this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }
    this.submitting = true;

    const payload = this.form.value as Omit<Grade, 'id'>;
    const userId = Number(this.userStore.getUserId());

    const obs = this.gradeToEdit
      ? this.gradeService.updateGrade(this.gradeToEdit.id, payload)
      : this.gradeService.createGradeForUser(userId, payload);

    obs.subscribe({
      next: () => {
        this.submitting = false;
        this.form.reset();
        this.gradeToEdit = undefined;
        this.changed.emit();
      },
      error: () => {this.submitting = false;}
    });
  }
}
