import {Component, Input, SimpleChanges} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {TaskService} from '../../services/task.service';
import {futureDateValidation} from '../../../../common_validators/fututreDateValidation';
import {Task} from '../../interfaces/task'
import {NgClass} from '@angular/common';

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
  constructor(private fb: FormBuilder,private taskService: TaskService) {
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
    this.taskService.createTaskForUser(1, formValue)
    this.taskForm.reset();
  }

}
