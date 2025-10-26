import {BehaviorSubject, catchError, map, Observable, tap, throwError} from 'rxjs';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Task} from '../../interfaces/task';
import {RelatedTasks} from '../../interfaces/related-tasks';
@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private taskSubject = new BehaviorSubject<Task[]>([]);
  tasks$ = this.taskSubject.asObservable();
  private readonly apiUrl = 'api/Task';

  constructor(private http: HttpClient) { }

  //Getters
  getAllUserTask(id: number): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/user/${id}`)
      .pipe(map(userTasks=>{
        this.taskSubject.next(userTasks);
        return this.taskSubject.getValue();
      }));
  }

  getAllGroupTask(id: number): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/group/${id}`)
      .pipe(map(groupTasks=>{
        this.taskSubject.next(groupTasks);
        return this.taskSubject.getValue();
      }));
  }

  getRelatedTasksByTaskId(id: number): Observable<RelatedTasks> {
    return this.http.get<RelatedTasks>(`${this.apiUrl}/${id}/relations`);
  }

  getTask(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`);
  }

  //Posts
  createTaskForUser(userId: number, task: Task): Observable<Task> {
    return this.http.post<Task>(`${this.apiUrl}/user/add/${userId}`, task)
      .pipe(map(newTask => {
        const currentTasks = this.taskSubject.getValue();
        this.taskSubject.next([...currentTasks, newTask]);
        return newTask;
      }));
  }

  createTaskForGroup(groupId: number, task: Task): Observable<Task> {
    return this.http.post<Task>(`${this.apiUrl}/group/add/${groupId}`, task);
  }

  createTaskRelation(taskId: number, relatedTaskId: number): Observable<Task> {
    return this.http.post<Task>(`${this.apiUrl}/${taskId}/relations`, { relatedTaskId });
  }

  //Deletes
  deleteTask(taskId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${taskId}`)
      .pipe(tap({
        next: () => {const currentTasks = this.
        taskSubject
          .getValue()
          .filter(task => task.id !== taskId);
          this.taskSubject.next(currentTasks);},
      }));
  }

  deleteTaskRelation(taskId: number, otherTaskId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${taskId}/relations/${otherTaskId}`);
  }

  //UpdatesTask
  updateTask(taskId: number, task: Task): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/${taskId}`, task)
      .pipe(map(updatedTask=>{
        const currentTasks = this.taskSubject.getValue();
        const updatedTasks = currentTasks.map(task => task.id === updatedTask.id ? updatedTask : task);
        this.taskSubject.next(updatedTasks);
        return updatedTask;
      }));
  }
}
