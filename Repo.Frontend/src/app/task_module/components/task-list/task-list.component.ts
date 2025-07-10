import { Component } from '@angular/core';
import { Task } from '../../interfaces/task'
@Component({
  selector: 'app-task-list',
  standalone: false,
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent {
    tasksList: Task[] = [
    {
      id: 1,
      Name: "Implementacja logowania",
      Description: "Stworzenie formularza logowania z walidacją",
      StartDate: new Date('2024-03-15'),
      EstimatedTime: 8,
      Creator: "Anna Kowalska",
      Deleted: false,
      Priority: "Wysoki",
      Status: "W trakcie"
    },
    {
      id: 2,
      Name: "Testy jednostkowe",
      Description: "Napisanie testów dla modułu użytkownika",
      StartDate: new Date('2024-03-16'),
      EstimatedTime: 5,
      Creator: "Jan Nowak",
      Deleted: false,
      Priority: "Średni",
      Status: "Nowy"
    }
  ];

    removeTask(task: Task) {
        this.tasksList = this.tasksList.filter(t => t !== task);
    }
}
