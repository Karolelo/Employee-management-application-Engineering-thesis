import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {Course} from '../../interfaces/course';
import {CourseService} from '../../services/course.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {UserMini} from '../../interfaces/user-mini';
import {catchError, of} from 'rxjs';

@Component({
  selector: 'app-course-enroll-overlay',
  standalone: false,
  templateUrl: './course-enroll-overlay.component.html',
  styleUrl: './course-enroll-overlay.component.css'
})
export class CourseEnrollOverlayComponent implements OnInit{
  @Input({required: true}) courseId!: number;
  @Output() close = new EventEmitter<boolean>();

  loading = true;
  submitting = false;
  course?: Course;
  isEnrolled = false;
  isExpired = false;
  error = '';
  isLeaderOrAdmin = false;
  editOpen = false;
  confirmOpen = false;

  constructor(
    private courseService: CourseService,
    private userStore: UserStoreService
  ) {
  }

  ngOnInit(): void {
    this.isLeaderOrAdmin = this.userStore.hasRole('TeamLeader') || this.userStore.hasRole('Admin');
    const userId = this.userStore.getUserId();
    this.courseService.getCourseById(this.courseId).subscribe({
      next: (course) => {
        this.course = course;
        const ends = this.parseDateOnly(course.finish_Date);
        const now = new Date();
        this.isExpired = !!ends && ends < now;

        this.courseService.getParticipants(course.id)
          .pipe(catchError(() => of([] as UserMini[])))
          .subscribe({
            next: (participant) => {
              this.isEnrolled = !!participant.find(p => p.id === userId);
              this.loading = false;
            },
            error: () => {this.loading = false}
          });
      },
      error: () => {
        this.error = 'Failed to load course.';
        this.loading = false;
      }
    });
  }

  enroll(): void {
    if (!this.course) return;
    this.submitting = true;
    this.courseService.enrollUser(this.courseId).subscribe({
      next: () => {
        this.isEnrolled = true;
        this.submitting = false;
        this.close.emit(true);
      },
      error: () => {this.submitting = false}
    });
  }

  unenroll(): void {
    if (!this.course) return;
    this.submitting = true;
    this.courseService.unenrollUser(this.courseId).subscribe({
      next: () => {
        this.isEnrolled = false;
        this.submitting = false;
        this.close.emit(true);
      },
      error: () => {this.submitting = false}
    });
  }

  dismiss(): void {
    this.close.emit(false);
  }

  openEdit(): void { this.editOpen = true; }
  onEditClosed(changed: boolean): void {
    this.editOpen = false;
    if (changed && this.course) {
      this.courseService.getCourseById(this.course.id).subscribe(c => this.course = c);
      this.close.emit(true);
    }
  }

  askDelete(): void { this.confirmOpen = true; }
  cancelDelete(): void { this.confirmOpen = false; }
  confirmDelete(): void {
    if (!this.course) return;
    this.submitting = true;
    this.courseService.deleteCourse(this.course.id).subscribe({
      next: () => { this.submitting = false; this.close.emit(true); },
      error: () => { this.submitting = false; },
      complete: () => this.confirmOpen = false
    });
  }

  private parseDateOnly(start: string): Date | null {
    if (!start) return null;
    const [y, m, d] = start
      .split('-')
      .map(Number);
    if (!y || !m || !d) return null;
    return new Date(y, m - 1, d);
  }

  formatDate(dateOnly: string): string {
    const d = this.parseDateOnly(dateOnly);
    if (!d) return dateOnly || '-';
    const dd = String(d.getDate()).padStart(2, '0');
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const yyyy = d.getFullYear();
    return `${dd}.${mm}.${yyyy}`;
  }
}
