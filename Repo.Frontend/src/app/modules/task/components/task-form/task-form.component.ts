import { Component } from '@angular/core';
import {AbstractControl, FormBuilder, ValidationErrors, ValidatorFn, Validators} from '@angular/forms';
import {TaskService} from '../../services/task.service';
import {futureDateValidation} from '../../../../common_validators/fututreDateValidation';

@Component({
  selector: 'app-task-form',
  standalone: false,
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css'
})
export class TaskFormComponent {
  constructor(private fb: FormBuilder,private taskService: TaskService) {
    fb.group({
      name:  ['', Validators.required],
      description: ['',Validators.compose([Validators.required, Validators.minLength(20)])],
      StartDate: [Date.now(),futureDateValidation],
      EstimatedTime: [0,Validators.compose([Validators.required, Validators.min(1)])],
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
        ...formValue,
        history: [
          ...this.taskToEdit.history,
          `Task "${this.taskToEdit.title}" updated on ${now}: ${this.generateUpdateLog(this.taskToEdit, formValue)}`
        ],
      };
      this.taskService.updateTask(updatedTask);
      this.taskToEdit = undefined;
    } else {
      const newTask: Task = {
        ...formValue,
        id: Date.now(),
        history: [`Task "${formValue.title}" created on ${now}`],
      };
      this.taskService.addTask(newTask);
    }
    this.taskForm.reset();
  }

}
