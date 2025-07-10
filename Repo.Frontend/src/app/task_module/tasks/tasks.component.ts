import {Component, inject} from '@angular/core';
import { TaskListComponent} from '../components/task-list/task-list.component';
import {Router} from '@angular/router';
@Component({
  selector: 'app-tasks',
  standalone: false,
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.css'
})
export class TasksComponent {
  router: Router = inject(Router)
  navigateToTaskCreator(){
    this.router.navigate(['/tasks/create']);
  }
}
