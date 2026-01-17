import {Component, OnInit, ViewChild,Pipe, PipeTransform} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {TaskService} from '../../services/task/task.service';
import {RelatedTasks} from '../../interfaces/related-tasks';
import {MatTableDataSource} from '@angular/material/table';
import {Task} from '../../interfaces/task';
import {MatPaginator} from '@angular/material/paginator';

@Component({
  selector: 'app-tasks-detail-page',
  standalone: false,
  templateUrl: './tasks-detail-page.component.html',
  styleUrl: './tasks-detail-page.component.css'
})
export class TasksDetailPageComponent implements OnInit{
  taskDetail!: RelatedTasks;
  displayedColumns: string[] = ['id','name','priority','status'];
  dataSource!: MatTableDataSource<Task>;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  constructor(private route: ActivatedRoute,private taskService: TaskService,private router: Router)
  {
  }
  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = params['id'];

      this.taskService.getRelatedTasksByTaskId(id).subscribe({
        next: (data: RelatedTasks) => {
           this.taskDetail = data;
           this.dataSource = new MatTableDataSource(this.taskDetail.relatedTasks);
         },
         error: (error) => {
           console.log(error);
         }
      }
      )
    })
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  onRowClick(row: Task) {
    this.router.navigate(['/tasks/task-details', row.id]);
  }
}
