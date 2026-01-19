import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  UserProfile,
  ChangeEmailRequest,
  ChangePasswordRequest,
} from '../interfaces/user-profile';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  constructor(private http: HttpClient) {}

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>('/api/userprofile/me');
  }

  changeEmail(payload: ChangeEmailRequest): Observable<any> {
    return this.http.put('/api/userprofile/me/email', payload);
  }

  changePassword(payload: ChangePasswordRequest): Observable<any> {
    return this.http.put('/api/userprofile/me/password', payload);
  }
}
