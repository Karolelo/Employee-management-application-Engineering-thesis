import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {GradeService} from '../../services/grade.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Grade} from '../../interfaces/grade';
import {finalize} from 'rxjs';

@Component({
  selector: 'app-grade-create-overlay',
  standalone: false,
  templateUrl: './grade-create-overlay.component.html',
  styleUrl: './grade-create-overlay.component.css'
})
export class GradeCreateOverlayComponent implements OnInit {
  @Input() userId?: number;
  @Input() grade?: Grade;
  @Output() close = new EventEmitter<boolean>();

  form!: FormGroup;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private gradeService: GradeService,
    private userStore: UserStoreService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      grade: [5, [Validators.required, Validators.min(0), Validators.max(5)]],
      description: ['', [Validators.required, Validators.maxLength(150)]],
      start_Date: ['', [Validators.required]],
      finish_Date: ['', [Validators.required]]
    }, { validators: this.finishAfterStartValidator });

    if (this.grade) {
      this.form.patchValue({
        grade: this.grade.grade,
        description: this.grade.description || '',
        start_Date: this.grade.start_Date,
        finish_Date: this.grade.finish_Date
      });
    }
  }

  private finishAfterStartValidator(group: AbstractControl): ValidationErrors | null {
    const s = group.get('start_Date')?.value;
    const f = group.get('finish_Date')?.value;
    if (!s || !f) return null;
    return f >= s ? null : { finishBeforeStart: true };
  }

  create(): void {
    if (this.form.invalid) return;
    this.submitting = true;
    const payload: Partial<Grade> = {
      grade: this.form.value.grade!,
      description: (this.form.value.description || '').trim(),
      start_Date: this.form.value.start_Date!,
      finish_Date: this.form.value.finish_Date!
    };
    const uid = this.userId ?? this.userStore.getUserId();
    const req$ = this.grade ? this.gradeService.updateGrade(this.grade.id, payload as Grade) : this.gradeService.createGradeForUser(uid!, payload as Grade);

    req$
      .pipe(finalize(() => {this.submitting = false;}))
      .subscribe({
        next: () => {},
        error: () => {console.log(this);},
        complete: () => {this.close.emit(true);}
      })
  }

  dismiss(): void { this.close.emit(false); }
}
