import { CanActivateFn } from '@angular/router';
import {inject} from '@angular/core'
import {UserStoreService} from '../../login/services/user_data/user-store.service';
import {UserService} from '../../dashboard/services/user/user.service';
import {TaskService} from '../services/task/task.service';
import {Task} from '../interfaces/task';
import {Observable} from 'rxjs'
export const taskGuard: CanActivateFn = async (route, state) => {

  //Getting task id from url
  const taskId = Number(route.paramMap.get('id'));

  if (!taskId || isNaN(taskId)) {
    return false; // Invalid ID
  }

  //Checking if user is admin then he can acces everything
  const userStoreService = inject(UserStoreService);
  const isAdmin = userStoreService.hasRole('Admin');

  if (isAdmin) {
    return true;
  }

  //Check if this task is assign to user
  const taskService = inject(TaskService)
  const userId = userStoreService.getUserId()

  if(userId) {
    const taskList = await taskService.getAllUserTask(userId).toPromise();

    if(taskList) {
      return taskList.some(t => t.id == taskId);
    }
  }

  return false;
};
