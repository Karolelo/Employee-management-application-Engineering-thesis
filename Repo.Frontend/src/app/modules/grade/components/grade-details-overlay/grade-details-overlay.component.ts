import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectorRef, NgZone } from '@angular/core';
import {Grade} from '../../interfaces/grade';
import {GradeService} from '../../services/grade.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-grade-details-overlay',
  standalone: false,
  templateUrl: './grade-details-overlay.component.html',
  styleUrl: './grade-details-overlay.component.css'
})
export class GradeDetailsOverlayComponent implements OnInit{
  @Input({required: true}) gradeId!: number;
  @Output() close = new EventEmitter<boolean>();

  loading = true;
  error = '';
  grade?: Grade;

  isLeaderOrAdmin=false; editOpen=false; confirmOpen=false; submitting=false;
  constructor(private gradeService: GradeService,
              private userStore: UserStoreService) { }

  ngOnInit(): void {
    this.isLeaderOrAdmin = this.userStore.hasRole('TeamLeader') || this.userStore.hasRole('Admin');
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
    this.close.emit(false);
  }

  openEdit(){ this.editOpen=true; }
  onEditClosed(changed:boolean){ this.editOpen=false; if(changed && this.grade){ this.gradeService.getGradeById(this.grade.id).subscribe(g=>this.grade=g); this.close.emit(true); } }
  askDelete(){ this.confirmOpen=true; }
  cancelDelete(){ this.confirmOpen=false; }
  confirmDelete(){
    if(!this.grade) return;
    this.submitting=true;
    this.gradeService.deleteGrade(this.grade.id).subscribe({
      next: ()=> { this.submitting=false; this.confirmOpen=false; this.close.emit(true); },
      error:()=> { this.submitting=false; this.confirmOpen=false; }
    });
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
