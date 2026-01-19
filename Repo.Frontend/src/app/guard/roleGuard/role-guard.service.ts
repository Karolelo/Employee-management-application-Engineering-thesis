import { Injectable } from '@angular/core';
import {ActivatedRouteSnapshot, CanActivateChild, Router} from '@angular/router';
import {UserStoreService} from '../../modules/login/services/user_data/user-store.service';
/*
  Basic guard for checking user permissions

 */
@Injectable({
  providedIn: 'root'
})
export class RoleGuardService implements CanActivateChild {
  constructor( private router: Router,private userStore: UserStoreService) {}
  canActivateChild(route: ActivatedRouteSnapshot): boolean {

    const expectedRole: string[] = route.data['expectedRoles'];

    if (!this.hasExpectedRole(expectedRole)
    ) {
      this.router.navigate(['/forbidden']);
      return false;
    }
    return true;
  }

  private hasExpectedRole(roles: string[]): boolean {
    return roles.some(role => this.userStore.hasRole(role));
  }

  private handleUnauthorizedAccess(){
    this.router.navigate(['/forbidden'])
  }
}
