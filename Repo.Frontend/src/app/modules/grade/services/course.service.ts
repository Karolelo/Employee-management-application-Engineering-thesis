import { Injectable } from '@angular/core';
import {Course, CourseMini} from '../interfaces/course';
import {BehaviorSubject, Observable, catchError, of, map, switchMap} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {UserMini} from '../interfaces/user-mini';

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private readonly apiUrl = 'api/Course';
  private courseSubject = new BehaviorSubject<Course[]>([]);
  courses$ = this.courseSubject.asObservable();

  constructor(private http: HttpClient) { }

  //[HttpGet] methods
  getCourses(q = '', page = 1, pageSize = 20): Observable<Course[]> {
    const url = `${this.apiUrl}?q=${(encodeURIComponent(q))}&page=${page}&pageSize=${pageSize}`;
    return this.http.get<Course[]>(url).pipe(
      catchError(() => of([])),
      map(list => list ?? []),
      map(list => {
        this.courseSubject.next(list);
        return list;
      })
    );
  }

  getCourseById(id: number): Observable<Course> {
    return this.http.get<Course>(`${this.apiUrl}/${id}`);
  }

  getParticipants(id: number): Observable<UserMini[]> {
    return this.http.get<UserMini[]>(`${this.apiUrl}/${id}/participants`).pipe(
      catchError(() => of([])),
      map(list => list ?? [])
    );
  }

  //[HttpPost] methods
  createCourse(body: CourseMini): Observable<Course> {
    return this.http.post<Course>(this.apiUrl, body).pipe(
      map(createdCourse => {
        this.courseSubject.next([createdCourse, ...this.courseSubject.getValue()]);
        return createdCourse;
      })
    );
  }

  enrollUser(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/enroll`, {});
  }

  //[HttpPut] methods
  updateCourse(id: number, body: CourseMini): Observable<Course> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, body).pipe(
      switchMap(() => this.getCourseById(id)),
      map(updatedCourse => {
        const data = this.courseSubject
          .getValue()
          .map(c => (c.id === updatedCourse.id ? updatedCourse : c));
        this.courseSubject.next(data);
        return updatedCourse;
      })
    );
  }

  //[HttpDelete] methods
  deleteCourse(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      map(() => {
        this.courseSubject.next(this.courseSubject
          .getValue()
          .filter(c => c.id !== id)
        );
        return undefined;
      })
    );
  }

  unenrollUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/enroll`);
  }
}
