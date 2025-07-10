import { Component } from '@angular/core';

@Component({
  selector: 'app-task-creator',
  standalone: false,
  templateUrl: './task-creator.component.html',
  styleUrl: './task-creator.component.css'
})
export class TaskCreatorComponent {
  priorities = [{value: 'High'}
    ,{value: 'Medium'}
    ,{value: 'Low'}];

  selectedPriority!: string;
}
