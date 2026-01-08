import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserMini } from '../interfaces/user-mini';

@Injectable({
  providedIn: 'root'
})
export class WorktimeUserService {
  private readonly apiUrl = 'api/User';

  constructor(private http: HttpClient) {}

  getUsers(q?: string): Observable<UserMini[]> {
    let params = new HttpParams();
    if (q) params = params.set('q', q);

    return this.http.get<UserMini[]>(this.apiUrl, { params });
  }
}
