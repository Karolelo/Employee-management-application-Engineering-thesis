import { Component,OnInit } from '@angular/core';
import { ChartConfiguration,ChartOptions,ChartType } from 'chart.js';
import {TaskService} from '../../../task/services/task/task.service';
import {GroupService} from '../../services/group/group.service';
import {Task} from '../../../task/interfaces/task';
import { map, catchError, switchMap } from 'rxjs/operators';
import { of,forkJoin } from 'rxjs';
@Component({
  selector: 'app-task-stats',
  standalone: false,
  templateUrl: './task-stats.component.html',
  styleUrl: './task-stats.component.css'
})

export class TaskStatsComponent implements OnInit{

  taskList: Task[] = [];

  doughnutChartData: ChartConfiguration<'doughnut'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        backgroundColor: []
      }
    ]
  };

  doughnutChartOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        position: 'top'
      }
    }
  };

  constructor(private taskService: TaskService, private groupService: GroupService) {
  }

  ngOnInit() {
    this.groupService.getGroups().pipe(
      map(groups => groups.map(group => group.id)),
      switchMap(groupIds =>
        groupIds.length > 0
          ? forkJoin(groupIds.map(id => this.taskService.getAllGroupTasks(id)))
          : of([])
      ),
      map(tasksArrays => {
        const allTasks = tasksArrays.flat()
        return this.filterTasksByCurrentMonth(allTasks)
      }),
      catchError(error => {
        console.log('Error:', error);
        return of([]);
      })
    )
      .subscribe(tasks => {
        console.log(tasks)
        this.taskList = tasks;
        this.updateChartData(tasks);
      });
  }

  private filterTasksByCurrentMonth(tasks: Task[]): Task[] {
    const date = new Date();
    const currentYear = date.getFullYear();
    const currentMonth = date.getMonth();

    return tasks
      .filter(task => new Date(task.start_Time).getMonth() === currentMonth
        && new Date(task.start_Time).getFullYear() == currentYear)
  }

  private updateChartData(tasks: Task[]) {
    // Policz taski po statusach
    const statusCount = new Map<string, number>();

    tasks.forEach(task => {
      const count = statusCount.get(task.status) || 0;
      statusCount.set(task.status, count + 1);
    });

    // Konwertuj Map na tablice
    const labels = Array.from(statusCount.keys());
    const data = Array.from(statusCount.values());

    console.log(labels);
    console.log(data);

    // Zaktualizuj dane wykresu
    this.doughnutChartData = {
      labels: labels,
      datasets: [
        {
          data: data,
          backgroundColor: this.getColors(labels.length)
        }
      ]
    };
  }

  private getColors(count: number): string[] {
    const colors = ['#f30707', '#0707FF', '#07f307', '#f3f307', '#f307f3', '#07f3f3'];
    return colors.slice(0, count).length > 0 ? colors.slice(0, count) : colors;
  }
}
