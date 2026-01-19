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
    this.isAdmin = this.userStore.hasRole('Admin');
  }

  onEditEntry(entry: WorkEntry): void {
    this.selectedEntry = entry;
  }

  onEntrySaved(): void {
    this.selectedEntry = undefined;
  }
}
