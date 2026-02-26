import { Injectable } from '@angular/core';
import {Group} from '../interfaces/group';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import {GroupService} from '../services/group/group.service';
import {Observable} from 'rxjs'
@Injectable({
  providedIn: 'root'
})
export class GroupEditResolverService implements Resolve<Group>{

  constructor(private group_service: GroupService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Group> {
    const groupId = Number(route.paramMap.get('id'));
    return this.group_service.getGroup(groupId);
  }
}
