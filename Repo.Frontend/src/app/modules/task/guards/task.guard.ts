import { CanActivateFn,Router } from '@angular/router';
import { inject } from '@angular/core';
import { UserStoreService } from '../../login/services/user_data/user-store.service';
import { TaskService } from '../services/task/task.service';
import { GroupService } from '../../dashboard/services/group/group.service';
import { Task } from '../interfaces/task';

const extractTaskId = (route: any): number | null => {
  const taskId = Number(route.paramMap.get('id'));
  return !taskId || isNaN(taskId) ? null : taskId;
};

const isUserAdmin = (userStoreService: UserStoreService): boolean => {
  return userStoreService.hasRole('Admin');
};

const checkUserTasks = async (
  userId: number,
  taskId: number,
  taskService: TaskService
): Promise<boolean> => {
  const taskList = await taskService.getAllUserTasks(userId).toPromise();
  return taskList ? taskList.some(t => t.id === taskId) : false;
};

const checkGroupTasks = async (
  userId: number,
  taskId: number,
  taskService: TaskService,
  groupService: GroupService
): Promise<boolean> => {
  const userGroups = await groupService.getUserGroups(userId).toPromise();

  if (!userGroups || userGroups.length === 0) {
    return false;
  }

  const userGroupTasksList: Task[] = [];

  for (const group of userGroups) {
    const groupTasks = await taskService.getAllGroupTasks(group.id).toPromise();
    if (groupTasks) {
      userGroupTasksList.push(...groupTasks);
    }
  }

  return userGroupTasksList.some(t => t.id === taskId);
};

export const taskGuard: CanActivateFn = async (route, state) => {
  const userStoreService = inject(UserStoreService);
  const taskService = inject(TaskService);
  const groupService = inject(GroupService);
  const router = inject(Router)

  const taskId = extractTaskId(route);
  if (taskId === null) {
    return false;
  }

  if (isUserAdmin(userStoreService)) {
    return true;
  }

  const userId = userStoreService.getUserId();
  if (!userId) {
    return false;
  }

  if (await checkUserTasks(userId, taskId, taskService)) {
    return true;
  }
  if(await checkGroupTasks(userId, taskId, taskService, groupService)) {
    return true;
  }
  //I'm just navigate to different page
  router.navigate(['/forbidden'])
  return false;
};
