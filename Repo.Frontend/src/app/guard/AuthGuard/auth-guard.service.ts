import { Injectable } from '@angular/core';
import {AuthService} from '../../modules/login/services/auth_managment/auth.service';
import {CanActivate, Router} from '@angular/router';
import {tap} from 'rxjs';
/*
 Main auth guard, which required user to
 log in
*/
@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate{
  constructor(public auth: AuthService, public router: Router) {}
  canActivate() {
    return this.auth
      .isAuthenticated()
      .pipe(
        tap(isAuthenticated => {
          if(!isAuthenticated){
            this.router.navigate(['login']);
            return false;
          }
          return true;
        })
      )
  }
}
