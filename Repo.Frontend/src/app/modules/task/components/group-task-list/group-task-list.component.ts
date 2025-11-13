
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {TaskService} from '../../services/task/task.service';
import {Task} from '../../interfaces/task';
import {TaskDetailsComponent} from '../task-details/task-details.component';
import {Observable, tap} from 'rxjs';
import {Router} from '@angular/router';

@Component({
  selector: 'app-group-task-list',
  standalone: false,
  templateUrl: './group-task-list.component.html',
  styleUrl: './group-task-list.component.css'
})
export class GroupTaskListComponent implements OnInit {
  @Input() groupId!: number;
  @Input() isLeader: boolean = false; // only team lead can edit this
  @Output() editTask = new EventEmitter<Task>();

  loading = false;
  errorMessage?: string;
  tasks$!: Observable<Task[]>;

  isTransparent = true;
  isCompact = true;
  constructor(
    private taskService: TaskService,
    private dialog: MatDialog,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.groupId) {
      this.errorMessage = 'Group ID is required';
      return;
    }

    this.loadGroupTasks();
  }

  private loadGroupTasks(): void {
    this.loading = true;
    this.tasks$ = this.taskService.tasks$;

    this.taskService.getAllGroupTask(this.groupId).subscribe({
      next: (tasks) => {
        this.loading = false;
        console.log('Group tasks loaded:', tasks);
      },
      error: (error) => {
        if(error.error.message.includes('Group has no tasks'))
        {
          this.errorMessage = 'Group has no tasks'
        }else {
          console.error(error);
          this.loading = false;
          this.errorMessage = 'Failed to load group tasks';
        }
      }
    });
  }

  onEditTask(task: Task): void {
    if (this.isLeader) {
      this.editTask.emit(task);
    }
  }

  onDeleteTask(taskId: number): void {
    if (!this.isLeader) {
      return;
    }

    this.taskService.deleteTask(taskId).pipe(
      tap({
        next: (response) => console.log('Task deleted:', response),
        error: (error) => console.error('Error:', error)
      })
    ).subscribe({});
  }

  onViewTask(task: Task): void {
    this.dialog.open(TaskDetailsComponent, {
      data: task,
    });
  }

  onShowDetails(task: Task): void {
    this.router.navigate(['/tasks/task-details', task.id]);
  }

  setProgressBarValue(value: string): number {
    switch(value.toLowerCase()) {
      case 'to-do': return 25;
      case 'in-progress': return 50;
      case 'done': return 100;
      default: return 0;
    }
  }
}
