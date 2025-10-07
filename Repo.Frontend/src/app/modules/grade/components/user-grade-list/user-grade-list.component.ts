import {Component, OnInit} from '@angular/core';
import {Grade} from '../../interfaces/grade';
import {Observable, take} from 'rxjs';
import {GradeService} from '../../services/grade.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-user-grade-list',
  standalone: false,
  templateUrl: './user-grade-list.component.html',
  styleUrl: './user-grade-list.component.css'
})
export class UserGradeListComponent implements OnInit {
  grades$!: Observable<Grade[]>;
  loading = true;
  errorMessage = '';

  userName = 'User';
  userInitial = '?';

  constructor(
    private gradeService: GradeService,
    private userStore: UserStoreService,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.userStore.getUserData().pipe(take(1)).subscribe(userData => {
      const nickname = userData?.nickname.trim();
      this.userName = nickname || 'User';
      this.userInitial = (this.userName || '?').charAt(0).toUpperCase();
    });

    const id = this.userStore.getUserId();
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

  openDetails(grade: Grade){
    this.router.navigate(['/grades/grade-details', grade.id]);
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
