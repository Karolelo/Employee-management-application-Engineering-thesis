import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import { NgClass } from '@angular/common';
import {MatDialog} from '@angular/material/dialog';
import {TaskService} from '../../services/task/task.service';
import {Task} from '../../interfaces/task'
import {TaskDetailsComponent} from '../task-details/task-details.component';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Observable, tap,async} from 'rxjs';
import {Router} from '@angular/router';
import {User} from '../../../dashboard/interfaces/user';
import {UserService} from '../../../dashboard/services/user/user.service';
import {GroupService} from '../../../dashboard/services/group/group.service';
import {Group} from '../../../dashboard/interfaces/group';

@Component({
  selector: 'app-task-list',
  standalone: false,
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
  providers: []
})
export class TaskListComponent implements OnInit{
  enableEdit:boolean = true;
  loading:boolean = false;
  errorMessage: string | null = null;
  tasks$!: Observable<Task[]>;
  @Output() editTask = new EventEmitter<Task>();

  //variables for updating other users tasks
  selectedUsersId: number|undefined;
  groupUsers$!:Observable<User[]>;
  errorMessageUsers: string = ''
  isTeamLead:boolean = false;
  constructor(private taskService: TaskService,private userDataStore: UserStoreService, private dialog: MatDialog,
              private router: Router, private user_service: UserService,private group_service: GroupService) {
    if(userDataStore.hasRole('TeamLeader'))
    {
      this.isTeamLead = true;
      const adminId = this.userDataStore.getUserId();
      if(adminId)
        this.getUsersFromGroupByAdminId(adminId)
    }
  }
  ngOnInit(): void {
    this.loading = true;
    this.tasks$ = this.taskService.tasks$;
    this.userDataStore.getUserData().subscribe({
      next: (user) => {
        if (user) {
          this.taskService.getAllUserTasks(user.id).subscribe({
            next: (task) => {
                this.loading = false;
                console.log(task);
            },
            error: (error) => {
              console.log(error);
              this.loading = false;
            }
         })
        } else {
          this.loading = false;
          this.errorMessage = 'no data of user';
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Error during loading data of user: ' + error.message;
        console.error('Error during loading data of user:', error);
      }
    });
  }

  onEditTask(task: Task): void {
    this.editTask.emit(task);
  }

  onDeleteTask(taskId: number): void {
    this.taskService.deleteTask(taskId).pipe(
      tap({
        next: (response) => console.log('Odpowiedź z serwera:', response),
        error: (error) => console.error('Błąd:', error)
      })
    ).subscribe({});
  }

  onViewTask(task: Task): void {
    this.dialog.open(TaskDetailsComponent, {
      data: task,
    });
  }

  onShowDetails(task: Task): void {
    this.router.navigate(['/tasks/task-details', task.id]);
  }

  setProgressBarValue(value: string): number {
      switch(value.toLowerCase()) {
      case 'to-do' : return 25;
      case 'in-progress' : return 50;
      case 'done' : return 100;
      default: return 0;
    }
  }

  private getUsersFromGroupByAdminId(adminId:number)
  {
    this.group_service.getGroupByAdminId(adminId).subscribe({
        next: (group: Group) => {
          this.groupUsers$ = this.user_service.getUsersFromGroup(group.id);
        },
        error: (error: any) =>{
          if(error.status === 404)
          {
            this.errorMessageUsers = 'You not have any group'
          } else
          {
            this.errorMessageUsers = 'Error during taking users from group'
          }
        }
      }
    )
  }

}
