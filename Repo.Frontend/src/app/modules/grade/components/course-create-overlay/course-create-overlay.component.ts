import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, Validators, AbstractControl, ValidationErrors, FormGroup} from '@angular/forms';
import {CourseService} from '../../services/course.service';
import {Course, CourseMini} from '../../interfaces/course';

@Component({
  selector: 'app-course-create-overlay',
  standalone: false,
  templateUrl: './course-create-overlay.component.html',
  styleUrl: './course-create-overlay.component.css'
})
export class CourseCreateOverlayComponent implements OnInit {
  @Input() course?: Course;
  @Output() close = new EventEmitter<boolean>();

  form!: FormGroup;
  submitting = false;

  constructor(private fb: FormBuilder, private courseService: CourseService) {
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required]],
      start_Date: ['', [Validators.required]],
      finish_Date: ['', [Validators.required]]
    }, {validators: [this.finishAfterStartValidator] });

    if (this.course) {
      this.form.patchValue({
        name: this.course.name,
        description: this.course.description,
        start_Date: this.course.start_Date,
        finish_Date: this.course.finish_Date
      });
    }
  }

  private finishAfterStartValidator(group: AbstractControl): ValidationErrors | null{
    const start = group.get('start_Date')?.value;
    const finish = group.get('finish_Date')?.value;
    if (!start || !finish) return null;
    return finish >= start ? null : { finishBeforeStart: true};
  }

  create(): void {
    if (this.form.invalid) return;
    this.submitting = true;
    const body: CourseMini = this.form.getRawValue() as CourseMini;

    const done = () => { this.submitting = false; this.close.emit(true); };
    if (this.course) {
      this.courseService.updateCourse(this.course.id, body).subscribe({ next: done, error: () => this.submitting = false });
    } else {
      this.courseService.createCourse(body).subscribe({ next: done, error: () => this.submitting = false });
    }
  }

  dismiss(): void {this.close.emit(false);}
}
