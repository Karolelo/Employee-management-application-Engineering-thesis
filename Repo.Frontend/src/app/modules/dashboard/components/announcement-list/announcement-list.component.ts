import { Component,Input,Output,EventEmitter } from '@angular/core';
import {Announcement} from '../../interfaces/announcement';
import {AnnouncementService} from '../../services/announcement/announcement.service';
import { Observable } from 'rxjs';
@Component({
  selector: 'app-announcement-list',
  standalone: false,
  templateUrl: './announcement-list.component.html',
  styleUrl: './announcement-list.component.css'
})
export class AnnouncementListComponent {
  enableEdit = false;
  @Input() announcements$: Observable<Announcement[]> = new Observable<Announcement[]>();
  @Output() editAnnouncement: EventEmitter<Announcement> = new EventEmitter<Announcement>();
  @Output() deleteAnnouncement: EventEmitter<number> = new EventEmitter<number>();
  constructor(private announcement_service: AnnouncementService) { }
  onEditAnnoucement(annoucement: Announcement) {
    this.editAnnouncement.emit(annoucement);
  }

  onDeleteAnnoucement(id:number) {
    this.deleteAnnouncement.emit(id);
  }
}
