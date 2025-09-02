import {Injectable, OnInit} from '@angular/core';
import {JwtHelperService} from '@auth0/angular-jwt';
import {BehaviorSubject} from 'rxjs';
import {UserData} from '../../interafaces/user-data';
@Injectable({
  providedIn: 'root'
})

export class UserStoreService {
  private userData = new BehaviorSubject<UserData | null>(null);
  private basicTokenPath = 'auth_token'// Implemented this variable for checking
                                              // if user is still log in, because when
                                              // you manually change url service losing tracks
  constructor(private jwtHelper: JwtHelperService) {
    this.isStillLoggedIn();
  }

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
  getUserData() {
    return this.userData.asObservable();
  }

  getUserId(): number | null {
    return this.userData.value?.id || null;
  }

  hasRole(role: string): boolean {
    return this.userData.value?.roles.includes(role) || false;
  }

  //Method for checking if user is still log in
  //Because if I directly go into

  private isStillLoggedIn(): boolean {
    try {
      const token = localStorage.getItem(this.basicTokenPath);
      if (!token) return false;

      if (this.jwtHelper.isTokenExpired(token)) {
        this.clearUserData();
        return false;
      }

      if (!this.userData.value) {
        this.storeUserData(token);
      }

      return true;
    } catch (error) {
      console.error('Error durring :', error);
      return false;
    }
  }
  //Method for clearing user data
  clearUserData() {
    this.userData.next(null);
  }
}
