import {Component, OnInit, Output, EventEmitter, OnChanges, SimpleChanges, Input} from '@angular/core';
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
export class CourseListComponent implements OnInit, OnChanges {
  @Input() userId?: number;
  @Output() select = new EventEmitter<number>();
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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['userId'] && !changes['userId'].firstChange) this.reload();
  }

  reload(): void {
    const uId = this.userId ?? this.userStore.getUserId();
    this.loading = true;
    if (!uId) {
      this.loading = false;
      return;
    }

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
                const enrolled = !!participants.find(p => p.id === uId);
                const ends = this.parseDateOnly(c.finish_Date);
                const now = new Date();
                const expired = !!ends && ends < now;
                const status: CourseStatus =
                  enrolled ? 'participating'
                    : expired ? 'expired'
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

  onSelect(course: CourseVM) {this.select.emit(course.id)}

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
