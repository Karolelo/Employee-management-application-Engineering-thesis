import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable,forkJoin} from 'rxjs';
import {User} from '../interfaces/user';
@Injectable({
  providedIn: 'root',
})
export class UserService {
  private url = 'api/User';
  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]>{
    return this.http.get<User[]>(this.url)
  }

  getUsersFromMyGroup(){

  }

  deleteUser(id: number): Observable<void>{
    return this.http.delete<void>(this.url + '/' + id);
  }

  deleteUsers(ids: number[]): Observable<void[]> {
    const deleteObservables = ids.map(id => this.deleteUser(id));
    return forkJoin(deleteObservables);
  }
}
