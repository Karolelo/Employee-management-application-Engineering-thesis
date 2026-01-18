import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {futureDateValidation} from '../../../../common_validators/fututreDateValidation';
import {Task} from '../../interfaces/task';
import {TaskService} from '../../services/task/task.service';
import {firstValueFrom, Observable,of} from 'rxjs';
import {map,catchError} from 'rxjs/operators'
import {MatSnackBar} from'@angular/material/snack-bar'
@Component({
  selector: 'app-group-task-form',
  standalone: false,
  templateUrl: './group-task-form.component.html',
  styleUrl: './group-task-form.component.css'
})
export class GroupTaskFormComponent implements OnChanges {
  @Input() taskToEdit?: Task;
  @Input() groupId!: number;
  //@Input() groupMembers: Array<{id: number, name: string}> = []; // group users
  @Output() taskUpdated = new EventEmitter<void>();

  taskForm!: FormGroup;
  isTransparent = true;
  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private matSnackBar: MatSnackBar
  ) {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.taskForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', [Validators.required, Validators.minLength(20)]],
      start_Time: [Date.now(), futureDateValidation],
      estimated_Time: [0, [Validators.required, Validators.min(1)]],
      priority: ['', Validators.required],
      status: ['', Validators.required],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['taskToEdit'] && this.taskToEdit) {
      this.updateFormWithTaskData();
    }
    const control = this.taskForm.get('start_Time');
    if (!control) return;

    if (this.taskToEdit) {
      control.clearValidators();
    } else {
      control.setValidators(futureDateValidation);
    }

    control.updateValueAndValidity({ emitEvent: false });
  }

  private updateFormWithTaskData(): void {
    const formattedTask = {
      ...this.taskToEdit,
      start_Time: new Date(this.taskToEdit!.start_Time).toISOString().split('T')[0]
    };
    this.taskForm.patchValue(formattedTask);
  }

  async onSubmit(): Promise<void> {
    if (!this.taskForm.valid) {
      this.matSnackBar.open('Form has an error','Ok',{duration:3000})
      return;
    }

    const canAdd = await firstValueFrom(this.checkTaskLimit());
    if(!canAdd) return

    try {
      const formValue = this.taskForm.value;
      const date = new Date(formValue.start_Time.toLocaleString());

      if (this.taskToEdit) {
        await this.updateExistingTask(formValue, date);
      } else {
        await this.createNewTask(formValue, date);
      }

      this.resetFormState();
      this.taskUpdated.emit();
    } catch (error) {
      console.error('Error during saving group task:', error);
    }
  }

  private async updateExistingTask(formValue: any, date: Date): Promise<void> {
    const updatedTask: Task = {
      ...this.taskToEdit,
      ...formValue,
      start_Time: date,
    };

    await firstValueFrom(
      this.taskService.updateTask(this.taskToEdit!.id, updatedTask)
    );
  }

  private async createNewTask(formValue: any, date: Date): Promise<void> {
    const newTask: Task = {
      ...formValue,
      start_Time: date,
    };

    await firstValueFrom(
      this.taskService.createTaskForGroup(this.groupId, newTask)
    );
  }

  resetFormState(): void {
    this.taskForm.reset();
    this.taskToEdit = undefined;
  }

  checkTaskLimit(): Observable<boolean> {
    return this.taskService.tasks$.pipe(
      map(tasks => {
        if(tasks && Array.isArray(tasks) && tasks.length >= 7) {
          this.matSnackBar.open('Maximum amount task for group is 7', 'Ok', {duration: 3000});
          return false;
        }
        return true;
      }),
      catchError(error => {
        console.error('Error checking task limit:', error);
        return of(true);
      })
    );
  }
}
