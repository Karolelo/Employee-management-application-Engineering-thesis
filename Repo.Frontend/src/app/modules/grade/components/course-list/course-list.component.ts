import { Component, OnInit } from '@angular/core';
import {Course} from '../../interfaces/course';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {CourseService} from '../../services/course.service';
import {switchMap, of, forkJoin, map, catchError} from 'rxjs';
import {UserMini} from '../../interfaces/user-mini';

type CourseStatus = 'participating' | 'undecided' | 'expired';
interface CourseVM extends Course {status: CourseStatus;}

@Component({
  selector: 'app-course-list',
  standalone: false,
  templateUrl: './course-list.component.html',
  styleUrl: './course-list.component.css'
})
export class CourseListComponent implements OnInit {
  courses: CourseVM[] = [];
  loading = true;

  constructor(
    private courseService: CourseService,
    private userStore: UserStoreService,
  ) {
  }

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    const userId = this.userStore.getUserId();
    if (!userId) {
      this.loading = false;
      return;
    }
    this.loading = true;

    this.courseService.getCourses('', 1, 50).subscribe({
      next: (courses) => {

        if (!courses.length) {
          this.courses = [];
          this.loading = false;
          return
        }
        forkJoin(
          courses.map(c =>
            this.courseService.getParticipants(c.id).pipe(
              catchError (() => of([] as UserMini[])),
              map(participants => {
                const enrolled = !!participants.find(p => p.id === userId);
                const starts = this.parseDateOnly(c.start_Date);
                const now = new Date();
                const status: CourseStatus =
                  enrolled ? 'participating'
                    : (starts && starts < now) ? 'expired'
                      : 'undecided';
                return {...c, status} as CourseVM;
              })
            )
          )
        ).subscribe({
          next: vms => {
            this.courses = vms;
            this.loading = false;
          },
          error: () => {
            this.courses = [];
            this.loading = false;
          }
        });
      },
      error: () => {
        this.courses = [];
        this.loading = false;
      }
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
