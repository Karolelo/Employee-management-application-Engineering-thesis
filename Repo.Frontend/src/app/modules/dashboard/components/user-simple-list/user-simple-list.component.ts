import {Component, Input, OnChanges, SimpleChanges,ViewChild,AfterViewInit} from '@angular/core';
import {MatTableDataSource} from '@angular/material/table';
import {User} from '../../interfaces/user';
import {MatPaginator} from '@angular/material/paginator';
@Component({
  selector: 'app-user-simple-list',
  standalone: false,
  templateUrl: './user-simple-list.component.html',
  styleUrl: './user-simple-list.component.css'
})
export class UserSimpleListComponent implements OnChanges,AfterViewInit {

  @Input() users: User[] = [];
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  displayedColumns: string[] = ['Name','Surname','Email','Actions'];
  dataSource = new MatTableDataSource<User>([]);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['users']) {
      this.dataSource.data = this.users ?? [];
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }

  onViewUser(user: User): void {

    console.log('View user details', user);
  }
}
