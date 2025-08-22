import {Component, EventEmitter, Output} from '@angular/core';
import { NgClass } from '@angular/common';
import {MatDialog} from '@angular/material/dialog';
import {TaskService} from '../../services/task.service';
import {Task} from '../../interfaces/task'
import {Observable, Subject, takeUntil} from 'rxjs';
import {TaskDetailsComponent} from '../task-details/task-details.component';
@Component({
  selector: 'app-task-list',
  standalone: false,
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
  providers: [TaskService]
})
export class TaskListComponent {
  tasks$!: Observable<Task[]>;
  @Output() editTask = new EventEmitter<Task>();
  constructor(private taskService: TaskService, private dialog: MatDialog) {
  }
  //TODO potem pokminić nad przekazywaniem ID mojego usera
  ngOnInit(): void {
    this.tasks$ = this.taskService
      .getAllUserTask(1);
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
