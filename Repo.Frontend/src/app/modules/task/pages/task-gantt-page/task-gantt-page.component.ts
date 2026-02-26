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

  currentUserId!: number;
  selectedUser: { id: number; name: string } | null = null;

  userOverlayOpen = false;

  viewCenterDate = new Date();

  windowDays = 7;

  constructor(
    private taskService: TaskService,
    private userStore: UserStoreService
  ) {}

  get effectiveUserId(): number {
    return this.selectedUser?.id ?? this.currentUserId;
  }

  ngOnInit(): void {
    this.isAdminOrLeader =
      this.userStore.hasRole('Admin') || this.userStore.hasRole('TeamLeader');

    const id = this.userStore.getUserId();
    if (!id) {
      this.loading = false;
      this.error = 'No user id';
      return;
    }

    this.currentUserId = id;
    this.ganttTasks$ = this.taskService.ganttTasks$;

    this.reload();
  }

  reload(): void {
    const id = this.effectiveUserId;
    if (!id) return;

    this.loading = true;
    this.error = null;

    this.taskService.loadUserGanttTasks(id).subscribe({
      next: () => (this.loading = false),
      error: err => {
        console.error(err);
        this.error = 'User has no task yet.';
        this.loading = false;
      },
    });
  }

  setWindowDays(days: number): void {
    this.windowDays = days;
  }

  shiftDay(delta: number): void {
    const date = new Date(this.viewCenterDate);
    date.setDate(date.getDate() + delta);
    this.viewCenterDate = date;
  }

  centerToday(): void {
    this.viewCenterDate = new Date();
  }

  openUserOverlay(): void {
    this.userOverlayOpen = true;
  }

  onUserOverlayClosed(): void {
    this.userOverlayOpen = false;
  }

  onUserChanged(user: { id: number; name: string }): void {
    this.selectedUser = user.id === this.currentUserId ? null : user;
    this.userOverlayOpen = false;
    this.reload();
  }
}
