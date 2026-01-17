import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TaskService } from '../../services/task/task.service';
import { GanttTask } from '../../interfaces/gantt-task';
import { UserStoreService } from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-task-gantt-page',
  templateUrl: './task-gantt-page.component.html',
  styleUrl: './task-gantt-page.component.css',
  standalone: false,
})
export class TaskGanttPageComponent implements OnInit {
  ganttTasks$!: Observable<GanttTask[]>;
  loading = true;
  error: string | null = null;

  isAdminOrLeader = false;
  selectedUserId?: number;

  constructor(
    private taskService: TaskService,
    private userStore: UserStoreService
  ) {}

  ngOnInit(): void {
    this.isAdminOrLeader =
      this.userStore.hasRole('Admin') || this.userStore.hasRole('TeamLeader');

    const currentUserId = this.userStore.getUserId();
    if (!currentUserId) {
      this.loading = false;
      this.error = 'No user id';
      return;
    }

    this.selectedUserId = currentUserId;
    this.ganttTasks$ = this.taskService.ganttTasks$;

    this.reload();
  }

  reload(): void {
    if (!this.selectedUserId) return;
    this.loading = true;
    this.error = null;

    this.taskService.loadUserGanttTasks(this.selectedUserId).subscribe({
      next: () => (this.loading = false),
      error: err => {
        console.error(err);
        this.error = 'Failed to load gantt tasks.';
        this.loading = false;
      },
    });
  }

  userOverlayOpen = false;

  openUserOverlay(): void {
    this.userOverlayOpen = true;
  }

  onUserOverlayClosed(): void {
    this.userOverlayOpen = false;
  }

  onUserChanged(user: { id: number; name: string }): void {
    this.selectedUserId = user.id;
    this.userOverlayOpen = false;
    this.reload();
  }
}
