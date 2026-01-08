import { Injectable } from '@angular/core';
import {BehaviorSubject, map, Observable} from 'rxjs';
import {Grade} from '../interfaces/grade';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GradeService {
  private readonly apiUrl = 'api/Grade';
  private gradeSubject = new BehaviorSubject<Grade[]>([]);
  grades$ = this.gradeSubject.asObservable();

  constructor(private http: HttpClient) { }

  //[HttpGet] methods
  getUserGrades(userId: number): Observable<Grade[]> {
    return this.http.get<Grade[]>(`${this.apiUrl}/user/${userId}`).pipe(
      map(list => {
        this.gradeSubject.next(list);
        return list;
      })
    );
  }

  getGradeById(id: number): Observable<Grade> {
    return this.http.get<Grade>(`${this.apiUrl}/${id}`);
  }

  //[HttpPost] methods

  //using Omit because backend uses MiniDTO for creating grades (without ID)
  createGradeForUser(userId: number, body: Omit<Grade, 'id'>): Observable<Grade> {
    const payload = {
      grade: body.grade,
      description: body.description,
      start_Date: body.start_Date,
      finish_Date: body.finish_Date
    };

    return this.http.post<Grade>(`${this.apiUrl}/user/${userId}`, payload).pipe(
      map(createdGrade => {
        this.gradeSubject.next([createdGrade, ...this.gradeSubject.getValue()]);
        return createdGrade;
      })
    );
  }

  //[HttpPut] methods
  updateGrade(gradeId: number, body: Omit<Grade, 'id'>): Observable<Grade> {
    const payload = {
      grade: body.grade,
      description: body.description,
      start_Date: body.start_Date,
      finish_Date: body.finish_Date
    };

    return this.http.put<Grade>(`${this.apiUrl}/${gradeId}`, payload, { observe: 'response' }).pipe(
      map(() => {
        const merged: Grade = {id: gradeId, ...payload} as Grade;
        const data = this.gradeSubject.getValue().map(it => it.id === gradeId ? {...it, ...merged } : it);
        this.gradeSubject.next(data);
        return merged;
      })
    );
  }

  //[HttpDelete] methods
  deleteGrade(gradeId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${gradeId}`).pipe(
      map(() => {
        this.gradeSubject.next(
          this.gradeSubject.getValue().filter(it => it.id !== gradeId)
        );
        return undefined;
      })
    );
  }
}
