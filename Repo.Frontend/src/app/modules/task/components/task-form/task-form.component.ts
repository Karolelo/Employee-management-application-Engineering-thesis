import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {futureDateValidation} from '../../../../common_validators/fututreDateValidation';
import {Task} from '../../interfaces/task'
import {NgClass} from '@angular/common';
import {TaskService} from '../../services/task/task.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-task-form',
  standalone: false,
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css'
})

export class TaskFormComponent implements OnChanges{
  @Input() taskToEdit?: Task;
  taskForm: FormGroup;
  constructor(private fb: FormBuilder,private taskService: TaskService,private userDataStore: UserStoreService) {
    this.taskForm=fb.group({
      name:  ['', Validators.required],
      description: ['',Validators.compose([Validators.required, Validators.minLength(20)])],
      startDate: [Date.now(),futureDateValidation],
      estimatedTime: [0,Validators.compose([Validators.required, Validators.min(1)])],
      priority: ['',Validators.required],
      status: ['',Validators.required]
    })
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['taskToEdit'] && this.taskToEdit) {
      this.taskForm.patchValue(this.taskToEdit);
    }
  }

  onSubmit(): void {
    const formValue = this.taskForm.value;

    if (this.taskToEdit) {

      const dateString = formValue.startDate.toLocaleString();
      const date = new Date(dateString);

      const updatedTask: Task = {
        ...this.taskToEdit,
        ...formValue,
        start_Time: date,
      };

      this.taskService.updateTask(this.taskToEdit.id, updatedTask).subscribe(
        {
          next: (response) => {
            console.log('Task updated:', response);
          },
          error: (error) => {
            console.error('Error durring updating task:', error);
          },
          complete: () => {
            this.taskForm.reset();
          }
        }
      );

      this.taskToEdit = undefined;

    } else {

      const dateString = formValue.startDate.toLocaleString();
      const date = new Date(dateString);

      const newTask: Task = {
        ...formValue,
        start_Time: date,
      };

      const userId = this.userDataStore.getUserId()

      if(userId) {
        this.taskService.createTaskForUser(userId, newTask).subscribe({
          next: (response) => {
            console.log('Task dodany:', response);
          },
          error: (error) => {
            console.error('Error durring adding task:', error);
          },
          complete: () => {
            this.taskForm.reset();
          }
        });
      }

    }
  }

}


