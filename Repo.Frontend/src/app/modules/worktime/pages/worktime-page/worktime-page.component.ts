import { Component, OnInit } from '@angular/core';
import { WorkEntry } from '../../interfaces/work-entry';
import { UserStoreService } from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-worktime-page',
  standalone: false,
  templateUrl: './worktime-page.component.html',
  styleUrls: ['./worktime-page.component.css']
})
export class WorktimePageComponent implements OnInit {
  selectedEntry?: WorkEntry;
  isAdmin = false;

  constructor(private userStore: UserStoreService) {}

  ngOnInit(): void {
    this.userStore.getUserData().subscribe({
      next: user => {
        if (!user) {
          this.isAdmin = false;
          return;
        }

        const roles: string[] = user.roles ?? [];
        this.isAdmin = roles.includes('Admin');
      },
      error: err => {
        console.error('Error loading user data', err);
        this.isAdmin = false;
      }
    });
  }

  onEditEntry(entry: WorkEntry): void {
    this.selectedEntry = entry;
  }

  onEntrySaved(): void {
    this.selectedEntry = undefined;
  }
}
