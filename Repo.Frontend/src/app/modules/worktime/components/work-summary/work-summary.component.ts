import { Component, OnInit } from '@angular/core';
import { WorkSummary } from '../../interfaces/work-summary';
import { WorkEntryService } from '../../services/work-entry.service';
import { UserStoreService } from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-work-summary',
  standalone: false,
  templateUrl: './work-summary.component.html',
  styleUrls: ['./work-summary.component.css']
})
export class WorkSummaryComponent implements OnInit {
  summary?: WorkSummary;
  loading = false;
  errorMessage: string | null = null;

  currentYear = new Date().getFullYear();
  currentMonth = new Date().getMonth() + 1;

  selectedYear = this.currentYear;
  selectedMonth = this.currentMonth;

  private userId?: number;

  constructor(
    private workEntryService: WorkEntryService,
    private userStore: UserStoreService
  ) {}

  ngOnInit(): void {
    this.userStore.getUserData().subscribe({
      next: user => {
        if (!user) {
          this.errorMessage = 'No user data available';
          return;
        }
        this.userId = user.id;
        this.loadSummary();
      },
      error: err => {
        this.errorMessage = 'Error loading user data';
        console.error(err);
      }
    });
  }

  loadSummary(): void {
    if (!this.userId) return;

    this.loading = true;
    this.errorMessage = null;

    this.workEntryService.getUserMonthlySummary(this.userId, this.selectedYear, this.selectedMonth)
      .subscribe({
        next: summary => {
          this.summary = summary;
          this.loading = false;
        },
        error: err => {
          this.loading = false;
          this.errorMessage = err.error?.message || 'Error loading summary';
          console.error(err);
        }
      });
  }

  onMonthChange(delta: number): void {
    let month = this.selectedMonth + delta;
    let year = this.selectedYear;

    if (month < 1) {
      month = 12;
      year--;
    } else if (month > 12) {
      month = 1;
      year++;
    }

    this.selectedMonth = month;
    this.selectedYear = year;
    this.loadSummary();
  }

  formatMonth(): string {
    return `${this.selectedYear}-${this.selectedMonth.toString().padStart(2, '0')}`;
  }
}
