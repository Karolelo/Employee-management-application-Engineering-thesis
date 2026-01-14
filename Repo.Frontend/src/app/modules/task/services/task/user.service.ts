import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { UserMini} from '../../interfaces/user-mini';
import {catchError, map, Observable, of} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = 'api/User';
  constructor(private http: HttpClient) { }

  getUsers(q = '', page = 1, pageSize = 20): Observable<UserMini[]> {
    const url = `${this.apiUrl}?q=${encodeURIComponent(q)}&page=${page}&pageSize=${pageSize}`;
    return this.http.get<UserMini[]>(url).pipe(
      catchError(() => of([])),
      map(data => data ?? [])
    );
  }

  getUserById(id: number): Observable<UserMini | null> {
    return this.http.get<UserMini>(`${this.apiUrl}/${id}`).pipe(
      catchError(() => of(null))
    );
  }
}
