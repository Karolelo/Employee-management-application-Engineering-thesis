import { Component,inject,ViewChild,AfterViewInit } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import {map} from 'rxjs/operators';
import {AnnouncementService} from '../../services/announcement/announcement.service';
import {Announcement} from '../../interfaces/announcement';
import {ActivatedRoute} from '@angular/router'
import {Observable} from 'rxjs';
import {AnnouncementListComponent} from '../../components/announcement-list/announcement-list.component';
import {GroupService} from '../../services/group/group.service';
import {Group} from '../../interfaces/group';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {User} from '../../interfaces/user';
import {UserService} from '../../services/user/user.service';
import {Task} from '../../../task/interfaces/task';
@Component({
  selector: 'app-manage-group-page',
  standalone: false,
  templateUrl: './manage-group-page.component.html',
  styleUrl: './manage-group-page.component.css'
})
export class ManageGroupPageComponent implements AfterViewInit{
  private breakpointObserver = inject(BreakpointObserver);

  //services
  private announcement_service = inject(AnnouncementService);
  private group_service = inject(GroupService);
  private userStore = inject(UserStoreService);
  private user_service = inject(UserService);

  //announcement part
  @ViewChild("listAnnouncement") announcementList!: AnnouncementListComponent;
  announcements$: Observable<Announcement[]> = new Observable<Announcement[]>();
  selectedAnnouncement?: Announcement;

  //group and task part
  group!: Group;
  groupUsers!: User[];
  taskToEdit?: Task;
  get canEditAdminId(): boolean { return this.userStore.hasRole('Admin'); }

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
        { title: 'Task management', cols: 2, rows: 4, type: 'taskStats' },
        { title: 'Group details', cols: 2, rows: 4, type: 'groupDetails'},
        { title: 'Users', cols: 1, rows: 1, type: 'usersView' }
      ];
    })
  );

  constructor(private route : ActivatedRoute) {
    const group_id = Number.parseInt(this.route.snapshot.paramMap.get('id')?.toString() ?? '0');

    this.announcement_service.getAnnouncementsForGroup(group_id).subscribe(
      () => {
        this.announcements$ = this.announcement_service.announcements$;
      },
      error => {
        console.log(error);
      }
    )

    this.group_service.getGroup(group_id).subscribe({
     next: (group: Group) => this.group = group,
     error: (error) => console.error('Error during getting group:', error)
    });

    this.user_service.getUsersFromGroup(group_id).subscribe({
        next: (users: User[]) => this.groupUsers = users,
        error: (error) => console.error('Error during getting users:', error)
      }
    )


  }
  ngAfterViewInit(){
    this.announcementList.enableEdit = true;
  }

  //Announcement part
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

  //Task part
  onTaskEdit(task: Task)
  {
    this.taskToEdit = task;
  }

  onTaskUpdated()
  {
    this.taskToEdit = undefined;
  }

}
