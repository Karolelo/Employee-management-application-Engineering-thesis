import {Component, Input, SimpleChanges} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
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
//TODO dodać potem dynamiczne priority oraz jak ID usera przekazywać
export class TaskFormComponent {
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
    if (this.taskForm.invalid) {
      alert('Please fill in the required fields: Title and Priority.');
      return;
    }
    const formValue = this.taskForm.value;
    const now = new Date().toLocaleString();
    if (this.taskToEdit) {
      const updatedTask: Task = {
        ...this.taskToEdit,
        ...formValue
      };
      this.taskService.updateTask(this.taskToEdit.id,updatedTask);
      this.taskToEdit = undefined;
    } else {
      const newTask: Task = {
        ...formValue,
      }

      const value = this.userDataStore.getUserId()
      if(value) {
        this.taskService.createTaskForUser(value,newTask);
      }
    }
    this.taskForm.reset();
    this.taskForm.reset();
  }

}
