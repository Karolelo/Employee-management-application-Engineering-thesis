import { Component,OnInit } from '@angular/core';
import {Task} from '../../interfaces/task'
import {TaskService} from '../../services/task/task.service';
import {ActivatedRoute} from '@angular/router';
@Component({
  selector: 'app-task-page',
  standalone: false,
  templateUrl: './task-page.component.html',
  styleUrl: './task-page.component.css'
})
export class TaskPageComponent implements OnInit {
  selectedTask?: Task;

  constructor(private taskService: TaskService, private route: ActivatedRoute) {}
  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const taskId = params['taskId'];
      if (taskId) {
        this.taskService.getTask(taskId).subscribe(task => {
          this.selectedTask = task;
        });
      }
    });
  }
  onEditTask(task: Task): void {
    this.selectedTask = task;
  }

  onTaskUpdated(): void {
    this.selectedTask = undefined;
  }
}
