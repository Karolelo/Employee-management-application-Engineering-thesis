import { Component,Input } from '@angular/core';
import {Announcement} from '../../interfaces/announcement';

@Component({
  selector: 'app-announcement-list',
  standalone: false,
  templateUrl: './announcement-list.component.html',
  styleUrl: './announcement-list.component.css'
})
export class AnnouncementListComponent {
  enableEdit = false;
  @Input() announcements: Announcement[] = [];
  constructor() { }
  onEditAnnoucement(annoucement: Announcement) {

  }

  onDeleteAnnoucement(id:number) {

  }
}
