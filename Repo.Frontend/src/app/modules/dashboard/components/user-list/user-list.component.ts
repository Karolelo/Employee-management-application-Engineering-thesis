import {AfterViewInit, Component, ViewChild} from '@angular/core';
import {MatTable} from '@angular/material/table';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {UserListDataSource} from './user-list-datasource';
import {User} from '../../interfaces/user';
import {MatDialog} from '@angular/material/dialog';
import {DeleteUserDialogComponent} from '../delete-user-dialog/delete-user-dialog.component';
import {UserService} from '../../services/user.service';
import {Router} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css',
  standalone: false
})
export class UserListComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<User>;
  dataSource!: UserListDataSource;
  selectedIds: number[] = [];

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['Nickname','Name','Surname','Login','Email','Role','Select'];

  constructor(private dialog: MatDialog,private userService: UserService,private router: Router,private snackBar: MatSnackBar) {
    this.dataSource = new UserListDataSource(userService)
  }
  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.table.dataSource = this.dataSource;
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter(value.trim().toLowerCase());
  }

  selectUser(id: number,event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;

    if(isChecked) {
      if(!this.selectedIds.includes(id)) {
        this.selectedIds.push(id);
      }
    } else {
      this.selectedIds = this.selectedIds.filter(x => x !== id);
    }
    console.log('Selected user id: ' + this.selectedIds);
  }

  onDelete() {
    const dialogRef = this.dialog.open(DeleteUserDialogComponent);
    dialogRef.afterClosed().subscribe(result => {
      if (result?.confirmed) {
        this.userService
          .deleteUsers(this.selectedIds).subscribe({
          next: () => {
            this.dataSource.removeUsers(this.selectedIds);
            this.selectedIds = [];
            },
          error: (error) => {
            console.error('Error durring delete:', error);
          }
          });
      }
    });
  }

  onAdd() {
    this.router.navigate(['/dashboard/users/create']);
  }

  onEdit() {
    if (this.selectedIds.length !== 1) {
      this.snackBar.open('You can edit only one user at a time', 'Close', {
        duration: 5000,
        panelClass: ['error-snackbar']
      });
    } else {
      this.router.navigate(['/dashboard/users/edit', this.selectedIds[0]])
    }
  }
}
