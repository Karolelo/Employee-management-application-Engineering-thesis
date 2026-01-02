import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { WorkEntry } from '../../interfaces/work-entry';
import { WorkEntryService } from '../../services/work-entry.service';
import { UserStoreService } from '../../../login/services/user_data/user-store.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-work-entry-list',
  standalone: false,
  templateUrl: './work-entry-list.component.html',
  styleUrls: ['./work-entry-list.component.css']
})
export class WorkEntryListComponent implements OnInit {
  @Input() isAdmin = false;
  @Output() editEntry = new EventEmitter<WorkEntry>();

  entries$!: Observable<WorkEntry[]>;
  loading = false;
  errorMessage: string | null = null;

  fromDate: string = '';
  toDate: string = '';

  availableUsers: { id: number; nickname: string }[] = [];
  selectedUserId: number | null = null;

  private userId?: number;

  constructor(
    private workEntryService: WorkEntryService,
    private userStore: UserStoreService
  ) { }

  ngOnInit(): void {
    this.entries$ = this.workEntryService.entries$;
    this.loadUserAndEntries();
  }

  private loadUserAndEntries(): void {
    this.loading = true;
    this.userStore.getUserData().subscribe({
      next: user => {
        if (!user) {
          this.loading = false;
          this.errorMessage = 'No user data available';
          return;
        }

        this.userId = user.id;
        this.reloadEntries();
      },
      error: err => {
        this.loading = false;
        this.errorMessage = 'Error loading user data';
        console.error(err);
      }
    });
  }

  reloadEntries(): void {
    const from = this.fromDate || undefined;
    const to = this.toDate || undefined;

    this.loading = true;

    if (this.isAdmin) {
      const userIdFilter = this.selectedUserId ?? undefined;

      this.workEntryService.getEntriesForAdmin(userIdFilter, from, to)
        .subscribe({
          next: (entries) => {
            this.loading = false;
            this.errorMessage = null;

            if (!userIdFilter) {
              this.buildAvailableUsers(entries);
            }
          },
          error: err => {
            this.loading = false;
            this.errorMessage = err.error?.message || 'Error loading work entries';
            console.error(err);
          }
        });
    } else {
      if (!this.userId) {
        this.loading = false;
        return;
      }

      this.workEntryService.getEntriesForUser(this.userId, from, to)
        .subscribe({
          next: () => {
            this.loading = false;
            this.errorMessage = null;
          },
          error: err => {
            this.loading = false;
            this.errorMessage = err.error?.message || 'Error loading work entries';
            console.error(err);
          }
        });
    }
  }

  private buildAvailableUsers(entries: WorkEntry[]): void {
    const map = new Map<number, string>();

    entries.forEach(e => {
      if (e.userID != null && e.userNickname) {
        map.set(e.userID, e.userNickname);
      }
    });

    this.availableUsers = Array.from(map.entries())
      .map(([id, nickname]) => ({ id, nickname }))
      .sort((a, b) => a.nickname.localeCompare(b.nickname));
  }

  clearFilter(): void {
    this.fromDate = '';
    this.toDate = '';
    if (this.isAdmin) {
      this.selectedUserId = null;
    }
    this.reloadEntries();
  }

  onEdit(entry: WorkEntry): void {
    this.editEntry.emit(entry);
  }

  onDelete(id: number): void {
    this.workEntryService.deleteEntry(id).subscribe({
      error: err => {
        console.error('Error deleting entry', err);
      }
    });
  }
}
