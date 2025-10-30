import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {Grade} from '../../interfaces/grade';
import {GradeService} from '../../services/grade.service';

@Component({
  selector: 'app-grade-details-overlay',
  standalone: false,
  templateUrl: './grade-details-overlay.component.html',
  styleUrl: './grade-details-overlay.component.css'
})
export class GradeDetailsOverlayComponent implements OnInit{
  @Input({required: true}) gradeId!: number;
  @Output() close = new EventEmitter<void>();

  loading = true;
  error = '';
  grade?: Grade;

  constructor(private gradeService: GradeService) {}

  ngOnInit(): void {
    this.loading = true;
    this.gradeService.getGradeById(this.gradeId).subscribe({
      next: grade => {
        this.grade = grade;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load grade details.';
        this.loading = false;
      }
    });
  }

  dismiss(): void {
    this.close.emit();
  }

  displayValuePercent(grade: Grade | undefined): string {
    if (!grade) {
      return '-';
    }
    const percent = ((grade.grade * 20) * 10) / 10;
    return `${percent}%`;
  }

  displayValue(grade: Grade | undefined): string {
    if (!grade) {
      return '-';
    }
    return grade.grade
      .toFixed(1)
      .replace('.0', '');
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '-';
    const [y, m, d] = dateStr.split('-').map(Number);
    if (!y || !m || !d) return dateStr;
    const pad = (n: number) => (n < 10 ? `0${n}` : `${n}`);
    return `${pad(d)}.${pad(m)}.${y}`;
  }
}
