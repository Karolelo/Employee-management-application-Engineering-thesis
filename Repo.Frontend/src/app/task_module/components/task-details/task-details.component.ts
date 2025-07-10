import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Task } from '../../interfaces/task';
//TODO zmienić CSS potem, bo mam tak statusy po polsku i zmieniam w stosunku do ich klase css
@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  standalone: false,
  styleUrls: ['./task-details.component.css']
})
export class TaskDetailsComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = ['name', 'status', 'actions'];
  relatedTasks: Task[] = [];
  currentTask?: Task;

  // TODO: Przenieść do serwisu
  private tasksList: Task[] = [
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

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.params
      .subscribe(params => {
        const taskId = Number(params['id']);
        this.loadTaskDetails(taskId);
      });
  }

  ngOnDestroy(): void {
  }

  onEdit(task?: Task): void {
    // TODO: Implementacja edycji
    console.log('Editing task:', task || this.currentTask);
  }

  onDelete(task?: Task): void {
    // TODO: Implementacja usuwania
    console.log('Deleting task:', task || this.currentTask);
  }

  private loadTaskDetails(taskId: number): void {
    this.currentTask = this.tasksList.find(task => task.id === taskId);
    this.relatedTasks = this.tasksList.filter(task => task.id !== taskId);
  }
}
