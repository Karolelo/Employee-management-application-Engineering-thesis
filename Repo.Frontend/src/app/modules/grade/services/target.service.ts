import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable, map, catchError, of, switchMap} from 'rxjs';
import {Target, TargetMini} from '../interfaces/target';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TargetService {
  private readonly apiUrl = 'api/Target';
  private targetSubject = new BehaviorSubject<Target[]>([]);
  targets$ = this.targetSubject.asObservable();

  constructor(private http: HttpClient) { }

  //[HttpGet] methods
  getUserTargets(userId: number): Observable<Target[]> {
    return this.http.get<Target[]>(`${this.apiUrl}/user/${userId}`).pipe(
      map(list => list ?? []),
      catchError(() => of([])),
      map(list => {
        this.targetSubject.next(list);
        return list;
      })
    );
  }

  getTargetById(id: number): Observable<Target> {
    return this.http.get<Target>(`${this.apiUrl}/${id}`)
  }

  //[HttpPost] methods
  createTargetForUser(userId: number, body: TargetMini): Observable<Target> {
    return this.http.post<Target>(`${this.apiUrl}/user/${userId}`, body).pipe(
      map(createdTarget => {
        this.targetSubject.next([createdTarget, ...this.targetSubject.getValue()]);
        return createdTarget;
      })
    );
  }

  //[HttpPut] methods
  updateTarget(id: number, body: TargetMini): Observable<Target> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, body).pipe(
      switchMap(() => this.getTargetById(id)),
      map(updatedTarget => {
        const data = this.targetSubject
          .getValue()
          .map(t => (t.id === updatedTarget.id ? updatedTarget : t));
        this.targetSubject.next(data);
        return updatedTarget;
      })
    );
  }

  //[HttpDelete] methods
  deleteTarget(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      map(() => {
        this.targetSubject.next(this.targetSubject
          .getValue()
          .filter(t => t.id !== id)
        );
        return undefined;
      })
    );
  }
}
