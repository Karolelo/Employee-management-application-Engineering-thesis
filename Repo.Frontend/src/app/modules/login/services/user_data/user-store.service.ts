import { Injectable } from '@angular/core';
import {JwtHelperService} from '@auth0/angular-jwt';
import {BehaviorSubject} from 'rxjs';
import {UserData} from '../../interafaces/user-data';
@Injectable({
  providedIn: 'root'
})
@Injectable({
  providedIn: 'root'
})
export class UserStoreService {
  private userData = new BehaviorSubject<UserData | null>(null);

  constructor(private jwtHelper: JwtHelperService) {}

  storeUserData(token: string) {
    try {
      const decodedToken = this.jwtHelper.decodeToken(token);
      console.log('Decode token:', decodedToken);
      const userData: UserData = {
        id: parseInt(decodedToken['id']),
        nickname: decodedToken['unique_name'],
        roles: this.extractRoles(decodedToken)
      };
      this.userData.next(userData);
      console.log('User data:', userData);
    } catch (error) {
      console.error('Error durring decoding:', error);
      this.userData.next(null);
    }
  }

  private extractRoles(decodedToken: any): string[] {
    const roleKey = 'role';
    const roles = decodedToken[roleKey];
    // changing for array

    if(Array.isArray(roles))
      return roles
    return [roles];
  }

  clearUserData() {
    this.userData.next(null);
  }

  getUserData() {
    return this.userData.asObservable();
  }

  hasRole(role: string): boolean {
    return this.userData.value?.roles.includes(role) || false;
  }
}
