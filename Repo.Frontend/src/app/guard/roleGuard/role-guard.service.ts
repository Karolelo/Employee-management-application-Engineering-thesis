import { Injectable } from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router} from '@angular/router';
import {AuthService} from '../../modules/login/services/auth_managment/auth.service';
import {UserStoreService} from '../../modules/login/services/user_data/user-store.service';
/*
  Basic guard for checking user permissions

 */
@Injectable({
  providedIn: 'root'
})
export class RoleGuardService implements CanActivate {
  constructor(private auth: AuthService, private router: Router,private userStore: UserStoreService) {}
  canActivate(route: ActivatedRouteSnapshot): boolean {
    // this will be passed from the route config
    // on the data property
    const expectedRole = route.data['expectedRole'];
    if (
      !this.auth.isAuthenticated() ||
      !this.userStore.hasRole(expectedRole)
    ) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }
}
