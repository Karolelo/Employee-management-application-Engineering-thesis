import {Component,ViewChild} from '@angular/core';
import {Announcement} from '../../interfaces/announcement';
import {Group} from '../../interfaces/group';
import {MatTableDataSource} from '@angular/material/table';
import {MatPaginator} from '@angular/material/paginator';
import {User} from '../../interfaces/user';
import {UserService} from '../../services/user/user.service';
import {ActivatedRoute,Router} from '@angular/router';
import {GroupService} from '../../services/group/group.service';
import {Observable} from 'rxjs'
import {AnnouncementService} from '../../services/announcement/announcement.service';
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


