import {Component,ViewChild} from '@angular/core';
import {MatTableDataSource} from '@angular/material/table';
import {MatPaginator} from '@angular/material/paginator';
import {Observable} from 'rxjs'
import {User} from '../../../dashboard/interfaces/user';
import {Announcement} from '../../../dashboard/interfaces/announcement';
import {Group} from '../../../dashboard/interfaces/group';
import {UserService} from '../../../dashboard/services/user/user.service';
import {GroupService} from '../../../dashboard/services/group/group.service';
import {AnnouncementService} from '../../../dashboard/services/announcement/announcement.service';
import {Router,ActivatedRoute} from '@angular/router'
@Component({
  selector: 'app-group-view-page',
  standalone: false,
  templateUrl: './group-view-page.component.html',
  styleUrl: './group-view-page.component.css'
})
export class GroupViewPageComponent {
   group!: Group;
   announcements$!: Observable<Announcement[]>;
   users: User[] = [];
   usersDataSource!: MatTableDataSource<any>;
   @ViewChild(MatPaginator) paginator!: MatPaginator;
   constructor(private user_service: UserService,
               private group_service: GroupService,
               private announcement_service: AnnouncementService,
               private activatedRoute: ActivatedRoute,
               private router: Router)
   {
     this.initializeValues();
   }

   initializeValues(): void {
     const group_id = Number.parseInt(this.activatedRoute.snapshot.paramMap.get('id') ?? '0');

     this.group_service
       .getGroup(group_id)
       .subscribe({
         next: (group: Group) => this.group = group,
         error: (error) => {
           if (error.status === 404) {
             this.router.navigate(['/404']);
           } else {
             console.error('Error:', error);
           }
         }
       });

     this.user_service.getUsersFromGroup(group_id).subscribe({
       next: (users: User[]) => {
         this.users = users;
       },
       error: (error) => console.error('Error getting users from group:', error)
     });

     this.announcements$ = this.announcement_service.getAnnouncementsForGroup(group_id);
   }
}


