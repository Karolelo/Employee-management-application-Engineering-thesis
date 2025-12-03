import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {futureDateValidation} from '../../../../common_validators/fututreDateValidation';
import {Task} from '../../interfaces/task'
import {NgClass} from '@angular/common';
import {TaskService} from '../../services/task/task.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {firstValueFrom, lastValueFrom, map, Observable, of} from 'rxjs';

@Component({
  selector: 'app-task-form',
  standalone: false,
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css'
})


export class TaskFormComponent implements OnChanges {

  tasksAvailableForRelation$!: Observable<Task[]>;
  tasksAvailableForRemoveRelation$!: Observable<Task[]>;

  @Input() taskToEdit?: Task;
  @Output() taskUpdated = new EventEmitter<void>();

  taskForm!: FormGroup;
  enableTaskRelations = false;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private userDataStore: UserStoreService
  ) {
    this.initializeForm();
    this.loadAvailableTasks();
  }


  private initializeForm(): void {
    this.taskForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', [Validators.required, Validators.minLength(20)]],
      start_Time: [Date.now(), futureDateValidation],
      estimated_Time: [0, [Validators.required, Validators.min(1)]],
      priority: ['', Validators.required],
      status: ['', Validators.required],
      tasksToConnect: [[]],
      connectedTasks: [[]]
    });
  }

  private loadAvailableTasks(): void {
    this.tasksAvailableForRelation$ = this.taskService.tasks$;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.shouldUpdateForm(changes)) {
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

  private shouldUpdateForm(changes: SimpleChanges): boolean {
    return changes['taskToEdit'] && this.taskToEdit !== undefined;
  }

  private updateFormWithTaskData(): void {
    const formattedTask = this.formatTaskData();
    this.taskForm.patchValue(formattedTask);

    this.setUpTasks();
  }

  private formatTaskData(): any {
    return {
      ...this.taskToEdit,
      start_Time: new Date(this.taskToEdit!.start_Time).toISOString().split('T')[0]
    };
  }

  private async updateAvailableTasks(): Promise<void> {
    const currentTasks = await firstValueFrom(this.tasksAvailableForRemoveRelation$);

    const taskIdsToSkip = new Set([
      ...currentTasks.map(task => task.id),
      ...(this.taskToEdit ? [this.taskToEdit.id] : [])
    ]);

    this.tasksAvailableForRelation$ = this.taskService.tasks$.pipe(
      map(tasks => tasks.filter(task => !taskIdsToSkip.has(task.id)))
    );
  }

  private setUpTasks(): void {
    this.taskService.getRelatedTasksByTaskId(this.taskToEdit!.id).subscribe({
      next: (relatedTasks) => {
        console.log(relatedTasks);
        const tasks = relatedTasks.relatedTasks;
        if(tasks.length > 0)
          this.tasksAvailableForRemoveRelation$ = of(tasks);
        else
          this.tasksAvailableForRemoveRelation$ = of([]);
      },complete: () => {
        this.updateAvailableTasks();
      },
      error: (error) => {
        console.error('Error durring loading tasks:', error);
      }
    });
  }

  // Managing of submit

  async onSubmit(): Promise<void> {
    if (!this.taskForm.valid) {
      console.error('Form has error');
      return;
    }

    try {
      const formValue = this.taskForm.value;
      const date = new Date(formValue.start_Time.toLocaleString());

      if (this.taskToEdit) {
        await this.updateExistingTask(formValue, date);
      } else {
        await this.createNewTask(formValue, date);
      }

      this.resetFormState();
    } catch (error) {
      console.error('Error durring saving tasks:', error);
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

    if(this.enableTaskRelations) {
      if(formValue.connectedTasks && formValue.connectedTasks.length>0)
        await this.deleteTaskRelations(this.taskToEdit!.id, formValue.connectedTasks);
      if(formValue.tasksToConnect && formValue.tasksToConnect.length>0)
        await this.createTaskRelations(this.taskToEdit!.id, formValue.tasksToConnect);
    }
    this.taskUpdated.emit();
  }

  private async createNewTask(formValue: any, date: Date): Promise<void> {
    const userId = this.userDataStore.getUserId();
    if (!userId) {
      throw new Error('No user with this id');
    }

    const creator_ID = this.userDataStore.getUserId()

    const newTask: Task = {
      ...formValue,
      start_Time: date,
      creator_ID: creator_ID
    };

    const createdTask = await firstValueFrom(
      this.taskService.createTaskForUser(userId, newTask)
    );

    if (this.enableTaskRelations && formValue.tasksToConnect) {
      await this.createTaskRelations(createdTask.id, formValue.tasksToConnect);
    }
  }

  private async createTaskRelations(taskId: number, selectedTaskIds: number[]): Promise<void> {
    const relationPromises = selectedTaskIds.map(relatedTaskId =>
      firstValueFrom(this.taskService.createTaskRelation(taskId, relatedTaskId))
    );

    await Promise.all(relationPromises);
  }

  private async deleteTaskRelations(taskId:number, selectedTaskIds: number[]): Promise<void> {
    const relationPromises = selectedTaskIds.map(relatedTaskId =>
      firstValueFrom(this.taskService.deleteTaskRelation(taskId, relatedTaskId))
    );

    await Promise.all(relationPromises);
  }


  protected resetFormState(): void {
    this.taskForm.reset();
    this.tasksAvailableForRelation$ = this.taskService.tasks$;
    this.tasksAvailableForRemoveRelation$ = of([]);
    this.taskToEdit = undefined;
    this.enableTaskRelations = false;
  }


}
