import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core'
import {UserStoreService} from '../../login/services/user_data/user-store.service';
import {GroupService} from '../../dashboard/services/group/group.service'
import {Router} from '@angular/router'
import {map} from 'rxjs'
import {Group} from '../../dashboard/interfaces/group';
export const groupGuard: CanActivateFn = (route, state) => {
  const userStoreService = inject(UserStoreService);
  const groupService = inject(GroupService);
  const router = inject(Router);

  const groupId = parseInt(route.paramMap.get('id') ?? '0', 10);
  const userId = userStoreService.getUserId();

  if (!userId || !groupId) {
    return router.createUrlTree(['myGroups']);
  }

  if(userStoreService.hasRole('Admin') || userStoreService.hasRole("TeamLeader"))
  {
    return true;
  }

  return groupService.getUserGroups(userId).pipe(
    map((groups: Group[]) =>
      groups.some(g => g.id === groupId)
        ? true
        : router.createUrlTree(['myGroups'])
    )
  );
};
