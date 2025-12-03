import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import { NgClass } from '@angular/common';
import {MatDialog} from '@angular/material/dialog';
import {TaskService} from '../../services/task/task.service';
import {Task} from '../../interfaces/task'
import {TaskDetailsComponent} from '../task-details/task-details.component';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Observable, tap,async} from 'rxjs';
import {Router} from '@angular/router';
import {User} from '../../../dashboard/interfaces/user';
import {UserService} from '../../../dashboard/services/user/user.service';
import {GroupService} from '../../../dashboard/services/group/group.service';
import {Group} from '../../../dashboard/interfaces/group';

@Component({
  selector: 'app-task-list',
  standalone: false,
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
  providers: []
})
export class TaskListComponent implements OnInit{
  enableEdit:boolean = true;
  loading:boolean = false;
  errorMessage: string | null = null;
  tasks$!: Observable<Task[]>;
  @Output() editTask = new EventEmitter<Task>();
  constructor(private taskService: TaskService,private userDataStore: UserStoreService, private dialog: MatDialog,
              private router: Router){
  }
  ngOnInit(): void {
    this.loading = true;
    this.tasks$ = this.taskService.tasks$;
    this.userDataStore.getUserData().subscribe({
      next: (user) => {
        if (user) {
          this.taskService.getAllUserTasks(user.id).subscribe({
            next: (task) => {
                this.loading = false;
                console.log(task);
            },
            error: (error) => {
              console.log(error);
              this.loading = false;
            }
         })
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
    this.taskService.deleteTask(taskId).pipe(
      tap({
        next: (response) => console.log('Odpowiedź z serwera:', response),
        error: (error) => console.error('Błąd:', error)
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
      case 'to-do' : return 25;
      case 'in-progress' : return 50;
      case 'done' : return 100;
      default: return 0;
    }
  }

}
