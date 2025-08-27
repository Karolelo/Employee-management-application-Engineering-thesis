import { Component } from '@angular/core';
import {TaskService} from '../../services/task.service';
import {Task} from '../../interfaces/task'
@Component({
  selector: 'app-task-page',
  standalone: false,
  templateUrl: './task-page.component.html',
  styleUrl: './task-page.component.css'
})
export class TaskPageComponent {
  selectedTask?: Task;

  constructor(private taskService: TaskService) {}

  onEditTask(task: Task): void {
    this.selectedTask = task;
  }

  onTaskUpdated(): void {
    this.selectedTask = undefined;
  }
}
