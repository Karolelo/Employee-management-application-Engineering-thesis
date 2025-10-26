import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {Group} from '../interfaces/group';
import {GroupService} from '../services/group/group.service';

@Injectable({
  providedIn: 'root'
})
//This is class for future development
//Problem with it redirecting our request omitting
//proxy soo it makes impossible on localhost to do it correcty
export class GroupResolverService implements Resolve<Group[]>{

  constructor(private groupService: GroupService,router: Router) { }
  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Group[]> {
    return this.groupService.getGroups().pipe(
      catchError((error) => {
        console.error('Error in resolver:', error);
        //this.router.navigate(['/error']); for now we not redirecting users
        //Maybe we will think about it
        return of([]);
      })
    );
  }
}
