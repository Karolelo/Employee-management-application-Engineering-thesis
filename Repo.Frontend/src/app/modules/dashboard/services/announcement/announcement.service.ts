import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Announcement} from '../../interfaces/announcement';
import {Observable, BehaviorSubject} from 'rxjs';
import {tap} from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private announcementSubject = new BehaviorSubject<Announcement[]>([]);
  announcements$ = this.announcementSubject.asObservable();
  private readonly baseUrl = 'api/Announcement/group';

  constructor(private http: HttpClient) { }

  getAnnouncementsForGroup(groupId: number): Observable<Announcement[]> {
    return this.http.get<Announcement[]>(`${this.baseUrl}/${groupId}`)
      .pipe(
        tap(announcements => {
          this.announcementSubject.next(announcements);
        })
      );
  }

  createAnnouncementForGroup(announcement: Announcement): Observable<Announcement> {
    return this.http.post<Announcement>(this.baseUrl, announcement)
      .pipe(
        tap(newAnnouncement => {
          const currentAnnouncements = this.announcementSubject.getValue();
          this.announcementSubject.next([...currentAnnouncements, newAnnouncement]);
        })
      );
  }

  updateAnnouncementForGroup(announcement: Announcement): Observable<Announcement> {
    return this.http.put<Announcement>(this.baseUrl, announcement)
      .pipe(
        tap(updatedAnnouncement => {
          const currentAnnouncements = this.announcementSubject.getValue();
          const updatedAnnouncements = currentAnnouncements.map(a =>
            a.id === updatedAnnouncement.id ? updatedAnnouncement : a
          );
          this.announcementSubject.next(updatedAnnouncements);
        })
      );
  }

  deleteAnnouncementForGroup(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
      .pipe(
        tap(() => {
          const currentAnnouncements = this.announcementSubject.getValue();
          this.announcementSubject.next(
            currentAnnouncements.filter(a => a.id !== id)
          );
        })
      );
  }

  hasReachedAnnouncementLimit(groupId: number): boolean {
    const currentAnnouncements = this.announcementSubject.getValue();
    const groupAnnouncements = currentAnnouncements.filter(a => a.group_ID === groupId);
    return groupAnnouncements.length >= 3;
  }

}
