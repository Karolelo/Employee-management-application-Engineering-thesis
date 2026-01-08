import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { WorkEntry } from '../interfaces/work-entry';
import { WorkSummary } from '../interfaces/work-summary';

@Injectable({
  providedIn: 'root'
})
export class WorkEntryService {
  private readonly apiUrl = 'api/WorkEntry';

  private entriesSubject = new BehaviorSubject<WorkEntry[]>([]);
  entries$ = this.entriesSubject.asObservable();

  constructor(private http: HttpClient) {}

  getEntriesForUser(userId: number, from?: string, to?: string): Observable<WorkEntry[]> {
    let params = new HttpParams();
    if (from) params = params.set('from', from);
    if (to) params = params.set('to', to);

    return this.http.get<WorkEntry[]>(`${this.apiUrl}/user/${userId}`, { params })
      .pipe(
        map(entries => {
          this.entriesSubject.next(entries);
          return entries;
        })
      );
  }

  getEntriesForAdmin(userId?: number, fromDate?: string, toDate?: string): Observable<WorkEntry[]> {
    let params = new HttpParams();
    if (userId != null) params = params.set('userId', userId);
    if (fromDate) params = params.set('from', fromDate);
    if (toDate) params = params.set('to', toDate);

    return this.http
      .get<WorkEntry[]>(this.apiUrl, { params })
      .pipe(
        map(entries => {
          this.entriesSubject.next(entries);
          return this.entriesSubject.getValue();
        })
      );
  }

  createEntryForUser(userId: number, entry: Partial<WorkEntry>): Observable<WorkEntry> {
    const payload = {
      work_Date: entry.work_Date,
      hours_Worked: entry.hours_Worked,
      task_ID: entry.task_ID ?? null,
      comment: entry.comment ?? null
    };

    return this.http.post<WorkEntry>(`${this.apiUrl}/user/${userId}`, payload)
      .pipe(
        map(created => {
          const current = this.entriesSubject.getValue();
          this.entriesSubject.next([...current, created]);
          return created;
        })
      );
  }

  updateEntry(id: number, entry: Partial<WorkEntry>): Observable<void> {
    const payload = {
      work_Date: entry.work_Date,
      hours_Worked: entry.hours_Worked,
      task_ID: entry.task_ID ?? null,
      comment: entry.comment ?? null
    };

    return this.http.put<void>(`${this.apiUrl}/${id}`, payload)
      .pipe(
        map(() => {
          const current = this.entriesSubject.getValue();
          const updated = current.map(e =>
            e.id === id ? { ...e, ...entry } as WorkEntry : e
          );
          this.entriesSubject.next(updated);
        })
      );
  }

  deleteEntry(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        map(() => {
          const current = this.entriesSubject.getValue();
          this.entriesSubject.next(current.filter(e => e.id !== id));
        })
      );
  }

  getUserMonthlySummary(userId: number, year: number, month: number): Observable<WorkSummary> {
    const params = new HttpParams()
      .set('year', year)
      .set('month', month);

    return this.http.get<WorkSummary>(`${this.apiUrl}/user/${userId}/summary`, { params });
  }
}
