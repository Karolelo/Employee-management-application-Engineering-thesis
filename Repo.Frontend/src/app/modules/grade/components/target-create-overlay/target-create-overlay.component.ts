import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {TargetService} from '../../services/target.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Target, TargetMini} from '../../interfaces/target';

@Component({
  selector: 'app-target-create-overlay',
  standalone: false,
  templateUrl: './target-create-overlay.component.html',
  styleUrl: './target-create-overlay.component.css'
})
export class TargetCreateOverlayComponent implements OnInit {
  @Input() userId?: number;
  @Input() target?: Target;
  @Output() close = new EventEmitter<boolean>();

  form!: FormGroup;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private targetService: TargetService,
    private userStore: UserStoreService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required]],
      start_Time: ['', [Validators.required]],
      finish_Time: [''],
      tag: ['']
    }, { validators: this.finishAfterStart });

    if (this.target) {
      const toLocal = (iso?: string|null) => {
        if (!iso) return '';
        const d = new Date(iso); if (isNaN(d.getTime())) return '';
        const pad=(n:number)=>n<10?'0'+n:''+n;
        return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
      };
      this.form.patchValue({
        name: this.target.name,
        description: this.target.description,
        start_Time: toLocal(this.target.start_Time),
        finish_Time: toLocal(this.target.finish_Time),
        tag: this.target.tag || ''
      });
    }
  }

  private finishAfterStart = (group: AbstractControl): ValidationErrors | null => {
    const s = group.get('start_Time')?.value;
    const f = group.get('finish_Time')?.value;
    if (!s || !f) return null;
    return f >= s ? null : { finishBeforeStart: true };
  };

  private toIso(dtLocal: string | null): string | null {
    if (!dtLocal) return null;
    const d = new Date(dtLocal);
    return isNaN(d.getTime()) ? null : d.toISOString();
  }

  create(): void {
    if (this.form.invalid) return;
    this.submitting = true;

    const dto: TargetMini = {
      name: this.form.value.name!,
      description: this.form.value.description!,
      start_Time: this.toIso(this.form.value.start_Time!)!,
      finish_Time: this.toIso(this.form.value.finish_Time || null),
      tag: this.form.value.tag || null
    };

    const uid = this.userId ?? this.userStore.getUserId();
    if (!uid) { this.submitting = false; return; }

    const done = () => { this.submitting = false; this.close.emit(true); };
    if (this.target) {
      this.targetService.updateTarget(this.target.id, dto).subscribe({ next: done, error: () => this.submitting = false });
    } else {
      this.targetService.createTargetForUser(uid, dto).subscribe({ next: done, error: () => this.submitting = false });
    }
  }

  dismiss(): void { this.close.emit(false); }
}
