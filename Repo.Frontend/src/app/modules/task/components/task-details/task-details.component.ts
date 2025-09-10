import {Component, Inject} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import {Task} from '../../interfaces/task'
import {Router} from '@angular/router';
@Component({
  selector: 'app-task-details',
  standalone: false,
  templateUrl: './task-details.component.html',
  styleUrl: './task-details.component.css'
})
export class TaskDetailsComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: Task,
              private dialogRef: MatDialogRef<TaskDetailsComponent>,private router: Router) {
  }
  close(){
    this.dialogRef.close();
  }
}
