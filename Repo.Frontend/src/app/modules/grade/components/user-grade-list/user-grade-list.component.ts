import {Component, OnInit, Output, EventEmitter, Input, OnChanges, SimpleChanges} from '@angular/core';
import {Grade} from '../../interfaces/grade';
import {Observable, take} from 'rxjs';
import {GradeService} from '../../services/grade.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Router} from '@angular/router';
import {UserService} from '../../services/user.service';

@Component({
  selector: 'app-user-grade-list',
  standalone: false,
  templateUrl: './user-grade-list.component.html',
  styleUrl: './user-grade-list.component.css'
})
export class UserGradeListComponent implements OnInit, OnChanges {
  @Input() userId?: number;
  @Input() usernameOverride?: string;
  @Output() select = new EventEmitter<number>();

  grades$!: Observable<Grade[]>;
  loading = true;
  errorMessage = '';

  userName = 'User';
  userInitial = '?';

  constructor(
    private gradeService: GradeService,
    private userStore: UserStoreService,
    private userService: UserService,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.load();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['userId'] || changes['usernameOverride']) this.load();
  }

  private load(): void {
    if (this.usernameOverride){
      this.applyName(this.usernameOverride);
    } else if (this.userId){
      this.userService.getUserById(this.userId).pipe(take(1)).subscribe(user => {
        this.applyName(user?.nickname.trim() || user?.login || 'User');
      });
    } else {
      this.userStore.getUserData().pipe(take(1)).subscribe(user => {
        this.applyName(user?.nickname.trim() || 'User');
      })
    }

    const id = this.userId ?? this.userStore.getUserId();
    if (!id) {
      this.loading = false;
      this.errorMessage = 'No user ID';
      return;
    }

    this.grades$ = this.gradeService.grades$;
    this.gradeService.getUserGrades(id).subscribe({
      next: () => (this.loading = false),
      error: () => {
        this.loading = false;
        this.errorMessage = 'Failed to load grades';
      }
    });
  }

  private applyName(name: string) {
    this.userName = name;
    this.userInitial = (name || '?').charAt(0).toUpperCase();
  }

  onSelect(grade: Grade) {
    this.select.emit(grade.id);
  }

  displayValue(grade: Grade): string {
    return grade.grade
      .toFixed(1)
      .replace('.0', '');
  }

  formatDate(dateStr: string): string {
    const [y, m, d] = dateStr.split('-').map(Number);
    if (!y || !m || !d) return dateStr;
    const pad = (n: number) => (n < 10 ? `0${n}` : `${n}`);
    return `${pad(d)}.${pad(m)}.${y}`;
  }
}
