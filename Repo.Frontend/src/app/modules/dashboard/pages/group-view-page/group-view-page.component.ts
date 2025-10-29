import {Component,AfterViewInit,ViewChild,OnInit} from '@angular/core';
import {Announcement} from '../../interfaces/announcement';
import {Group} from '../../interfaces/group';
import {NgOptimizedImage} from '@angular/common'
import {TaskModule} from '../../../task/task.module';
import {MatTableModule,MatTableDataSource} from '@angular/material/table';
import {MatPaginatorModule,MatPaginator} from '@angular/material/paginator';
import {User} from '../../interfaces/user';
import {UserService} from '../../services/user/user.service';
import {ActivatedRoute} from '@angular/router';
import {GroupService} from '../../services/group/group.service';
import {DomSanitizer,SafeUrl} from '@angular/platform-browser';
import {TaskListComponent} from '../../../task/components/task-list/task-list.component';
@Component({
  selector: 'app-group-view-page',
  standalone: false,
  templateUrl: './group-view-page.component.html',
  styleUrl: './group-view-page.component.css'
})
export class GroupViewPageComponent implements AfterViewInit{
  group!: Group;
   announcements: Announcement[] = []
   displayedColumns: string[] = ['Name','Surname','Email'];
   users: User[] = [];
   usersDataSource!: MatTableDataSource<any>;
   @ViewChild(MatPaginator) paginator!: MatPaginator;
   @ViewChild(TaskListComponent) taskList!: TaskListComponent;
   imageUrl: SafeUrl = "";
   constructor(private user_service: UserService,
               private group_service: GroupService,
               private activatedRoute: ActivatedRoute,
               private sanitizer: DomSanitizer)
   {
     this.initializeValues()
   }

   initializeValues(): void {
     const group_id =Number.parseInt(this.activatedRoute.snapshot.paramMap.get('id') ?? '0');
     this.group_service
       .getGroup(group_id)
       .subscribe({
         next: (group:Group) => this.group = group,
         error: (error) => console.error('Error getting group:', error)
       });

     this.group_service.getGroupImagePath(group_id).subscribe({
       next: (blob: Blob) => {
         const objectUrl = URL.createObjectURL(blob);
         this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(objectUrl);
         console.log(this.imageUrl);
       },
       error: (error) => {
         console.error('Błąd podczas pobierania obrazu:', error);
       }
     });

     this.user_service.getUsersFromGroup(group_id).subscribe({
       next: (users: User[]) => {
         this.users = users;
         this.usersDataSource = new MatTableDataSource(this.users);
         this.usersDataSource.paginator = this.paginator;
       },
       error: (error) => console.error('Error getting users from group:', error)
     });
   }
   ngAfterViewInit(): void {
     if (this.usersDataSource) {
       this.usersDataSource.paginator = this.paginator;
     }

     setTimeout(() => {
       if(this.taskList) {
         this.taskList.changeValueToGroupTask(
           this.group.id)
       }
     })
   }
}


