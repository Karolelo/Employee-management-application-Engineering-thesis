import { Component,OnInit,OnChanges,SimpleChanges } from '@angular/core';
import {Task} from '../../interfaces/task'
import {TaskService} from '../../services/task/task.service';
import {ActivatedRoute} from '@angular/router';
import {UserService} from '../../../dashboard/services/user/user.service';
import {GroupService} from '../../../dashboard/services/group/group.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Group} from '../../../dashboard/interfaces/group';
import {Observable} from 'rxjs'
import {User} from '../../../dashboard/interfaces/user';
@Component({
  selector: 'app-task-page',
  standalone: false,
  templateUrl: './task-page.component.html',
  styleUrl: './task-page.component.css'
})
export class TaskPageComponent implements OnInit,OnChanges {
  selectedTask?: Task;

  //Variables for editing other users tasks from the team leader group
  groupUsers$!: Observable<User[]>;
  isTeamLead: boolean = false
  selectedUserId?:number;
  errorMessageUsers?:string;
  constructor(private taskService: TaskService, private route: ActivatedRoute,
              private user_service: UserService,private group_service: GroupService,
              private userDataStore: UserStoreService){
    if(userDataStore.hasRole('TeamLeader')) {
      this.isTeamLead = true;
      const adminId = this.userDataStore.getUserId();
      if (adminId)
        this.getUsersFromGroupByAdminId(adminId)
    }
  }
  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const taskId = params['taskId'];
      if (taskId) {
        this.taskService.getTask(taskId).subscribe(task => {
          this.selectedTask = task;
        });
      }
    });
  }

  ngOnChanges(simpleChanges: SimpleChanges){
    if(simpleChanges['selectedUserId'] && this.selectedUserId !== undefined)
      this.selectedTask = undefined
  }
  onEditTask(task: Task): void {
    this.selectedTask = task;
  }

  onTaskUpdated(): void {
    this.selectedTask = undefined;
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
