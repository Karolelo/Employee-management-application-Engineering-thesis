import {catchError, Observable, throwError} from 'rxjs';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiResponse} from '../../../common_interafaces/api/api-response';
import {Task} from '../interfaces/task';
@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly apiUrl = '/api/Task';

  constructor(private http: HttpClient) { }
  //Getters
  getAllUserTask(id: number): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/user/${id}`)
      .pipe(catchError(this.handleError));
  }

  getAllGroupTask(id: number): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/group/${id}`)
      .pipe(catchError(this.handleError));
  }

  getRelatedTasksByTaskId(id: number): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/${id}/relations`)
      .pipe(catchError(this.handleError));
  }
  //Posts
  createTaskForUser(userId: number, task: Task): Observable<ApiResponse<Task>> {
    return this.http.post<ApiResponse<Task>>(`${this.apiUrl}/user/add/${userId}`, task)
      .pipe(catchError(this.handleError));
  }

  createTaskForGroup(groupId: number, task: Task): Observable<ApiResponse<Task>> {
    return this.http.post<ApiResponse<Task>>(`${this.apiUrl}/group/add/${groupId}`, task)
      .pipe(catchError(this.handleError));
  }

  createTaskRelation(taskId: number, otherTaskId: number): Observable<ApiResponse<Task>> {
    return this.http.post<ApiResponse<Task>>(`${this.apiUrl}/${taskId}/relations`, { otherTaskId })
      .pipe(catchError(this.handleError));
  }
  //Deletes
  deleteTask(taskId: number): Observable<ApiResponse<Task>> {
    return this.http.delete<ApiResponse<Task>>(`${this.apiUrl}/${taskId}`)
      .pipe(catchError(this.handleError));
  }

  deleteTaskRelation(taskId: number, otherTaskId: number): Observable<ApiResponse<Task>> {
    return this.http.delete<ApiResponse<Task>>(`${this.apiUrl}/${taskId}/relations/${otherTaskId}`)
  }
  private handleError(error: any) {
    console.error('Wystąpił błąd:', error);
    return throwError(() => new Error(error.message || 'Wystąpił błąd serwera'));
  }
}
