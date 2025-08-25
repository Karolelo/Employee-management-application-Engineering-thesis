import {Component, EventEmitter, Output} from '@angular/core';
import { NgClass } from '@angular/common';
import {MatDialog} from '@angular/material/dialog';
import {TaskService} from '../../services/task.service';
import {Task} from '../../interfaces/task'
import {Observable, Subject, takeUntil} from 'rxjs';
import {TaskDetailsComponent} from '../task-details/task-details.component';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import { tap, catchError, of,async } from 'rxjs';

@Component({
  selector: 'app-task-list',
  standalone: false,
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
  providers: [TaskService]
})
export class TaskListComponent {
  loading = false;
  errorMessage: string | null = null;
  tasks$!: Observable<Task[]>;
  @Output() editTask = new EventEmitter<Task>();
  constructor(private taskService: TaskService,private userDataStore: UserStoreService, private dialog: MatDialog) {
    /*console.log('User data store ');
    this.userDataStore.getUserData().subscribe(user => {
      console.log('User data store ', user);
    })*/
  }
  ngOnInit(): void {
    this.loading = true;
    this.userDataStore.getUserData().subscribe({
      next: (user) => {
        if (user) {

          this.tasks$ = this.taskService.getAllUserTask(user.id).pipe(
            tap(() => {
              this.loading = false;
              this.errorMessage = null;
            }),
            catchError((error) => {
              this.loading = false;
              this.errorMessage = 'Failed to load tasks: ' + error.message;
              console.error('Failed to load tasks: ', error);
              return of([]);
            })
          );
        } else {
          this.loading = false;
          this.errorMessage = 'no data of user';
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Error during loading data of user: ' + error.message;
        console.error('Error during loading data of user:', error);
      }
    });
  }

  onEditTask(task: Task): void {
    this.editTask.emit(task);
  }

  onDeleteTask(taskId: number): void {
    this.taskService.deleteTask(taskId);
  }

  onViewTask(task: Task): void {
    this.dialog.open(TaskDetailsComponent, {
      data: task,
    });
  }

}
