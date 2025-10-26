import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable,forkJoin} from 'rxjs';
import {User} from '../../interfaces/user';
@Injectable({
  providedIn: 'root',
})
export class UserService {
  private url = 'api/User';
  constructor(private http: HttpClient) { }

  getUser(id: number): Observable<User>{
    return this.http.get<User>(this.url+'/'+id);
  }
  getUsers(): Observable<User[]>{
    return this.http.get<User[]>(this.url)
  }
  getUsersFromGroup(groupId: number){
    return this.http.get<User[]>(this.url + '/group/' + groupId);
  }

  deleteUser(id: number): Observable<void>{
    return this.http.delete<void>(this.url + '/' + id);
  }

  deleteUsers(ids: number[]): Observable<void[]> {
    const deleteObservables = ids.map(id => this.deleteUser(id));
    return forkJoin(deleteObservables);
  }

  creteUser(data: any): Observable<User>{
    return this.http.post<User>('api/Auth/register',data);
  }

  updateUser(data: User): Observable<User>{
    return this.http.put<User>(this.url,data);
  }
}
