import { Component,inject,ViewChild,AfterViewInit } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import {map} from 'rxjs/operators';
import {AnnouncementService} from '../../services/announcement/announcement.service';
import {Announcement} from '../../interfaces/announcement';
import {ActivatedRoute} from '@angular/router'
import {Observable} from 'rxjs';
import {AnnouncementListComponent} from '../../components/announcement-list/announcement-list.component';
@Component({
  selector: 'app-manage-group-page',
  standalone: false,
  templateUrl: './manage-group-page.component.html',
  styleUrl: './manage-group-page.component.css'
})
export class ManageGroupPageComponent implements AfterViewInit{
  private breakpointObserver = inject(BreakpointObserver);
  private announcement_service = inject(AnnouncementService);
  announcements$: Observable<Announcement[]> = new Observable<Announcement[]>();
  group_id: number = 0;
  selectedAnnouncement?: Announcement;
  @ViewChild("listAnnouncement") announcementList!: AnnouncementListComponent;

  cards = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map(({ matches }) => {
      if (matches) {
        return [
          { title: 'Card 1', cols: 1, rows: 1,type: 'announcement' },
          { title: 'Card 2', cols: 1, rows: 2,type: 'taskStats' },
          { title: 'Card 3', cols: 1, rows: 3,type: 'groupsStats' },
        ];
      }

      return [
        { title: 'announcements', cols: 2, rows: 2, type: 'announcement' },
        { title: 'Task management', cols: 2, rows: 2, type: 'taskStats' },
        { title: 'Group details', cols: 1, rows: 1, type: 'groupDetails'},
        { title: 'Users', cols: 1, rows: 1, type: 'usersView' }
      ];
    })
  );

  constructor(private route : ActivatedRoute) {
    this.group_id = Number.parseInt(this.route.snapshot.paramMap.get('id')?.toString() ?? '0');

    this.announcement_service.getAnnouncementsForGroup(this.group_id).subscribe(
      () => {
        this.announcements$ = this.announcement_service.announcements$;
      },
      error => {
        console.log(error);
      }
    )
  }

  ngAfterViewInit(){
    this.announcementList.enableEdit = true;
  }

  onDeleteAnnouncement(announcementId: number) {
    this.announcement_service.deleteAnnouncementForGroup(announcementId).subscribe(
      () => {
        console.log('Announcement deleted successfully.');
      },
      error => {
        console.error('Error during deleting of the announcement:', error);
      }
    );
  }

  onEditAnnouncement(announcement: Announcement) {
   this.selectedAnnouncement = announcement;
  }

}
