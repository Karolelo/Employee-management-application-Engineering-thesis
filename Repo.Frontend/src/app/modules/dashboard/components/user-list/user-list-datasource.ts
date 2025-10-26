import { DataSource } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { map,filter } from 'rxjs/operators';
import { Observable, of as observableOf, merge } from 'rxjs';
import {User} from '../../interfaces/user';
import { BehaviorSubject,finalize,combineLatest } from 'rxjs';
import {UserService} from '../../services/user/user.service';

/**
 * Data source for the UserList view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
export class UserListDataSource extends DataSource<User> {

  private originalData: User[] = [];
  dataSubject: BehaviorSubject<User[]> = new BehaviorSubject<User[]>([]);
  private filterSubject: BehaviorSubject<string> = new BehaviorSubject<string>('');
  paginator: MatPaginator | undefined;
  sort: MatSort | undefined;
  constructor(private user_service: UserService) {
    super();
    this.loadUser();
    this.setupFilter()
  }

  loadUser(){
    this.user_service.getUsers().subscribe(
      (users) => {
        this.originalData = users;
        this.dataSubject.next(users);
      },
      (error) => {
        console.error('Error durring loading users:', error);
      },
      () => {
        console.log('Users loaded');
      }
    );
  }

  /**
   * Connect this data source to the table. The table will only update when
   * the returned stream emits new items.
   * @returns A stream of the items to be rendered.
   */
  connect(): Observable<User[]> {
    if (this.paginator && this.sort) {
      // Combine everything that affects the rendered data into one update
      // stream for the data-table to consume.
      return merge(this.dataSubject.asObservable(), this.paginator.page, this.sort.sortChange)
        .pipe(map(() => {
          return this.getPagedData(this.getSortedData([...this.dataSubject.value]));
        }));
    } else {
      throw Error('Please set the paginator and sort on the data source before connecting.');
    }
  }

  /**
   *  Called when the table is being destroyed. Use this function, to clean up
   * any open connections or free any held resources that were set up during connect.
   */
  disconnect(): void {
  }

  /**
   * Paginate the data (client-side). If you're using server-side pagination,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getPagedData(data: User[]): User[] {
    if (this.paginator) {
      const startIndex = this.paginator.pageIndex * this.paginator.pageSize;
      return data.splice(startIndex, this.paginator.pageSize);
    } else {
      return data;
    }
  }

  /**
   * Sort the data (client-side). If you're using server-side sorting,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getSortedData(data: User[]): User[] {
    if (!this.sort || !this.sort.active || this.sort.direction === '') {
      return data;
    }

    return data.sort((a, b) => {
      const isAsc = this.sort?.direction === 'asc';
      switch (this.sort?.active) {
        case 'Nickname':
          return compare(a.nickname, b.nickname, isAsc);
        case 'Name':
          return compare(a.name, b.name, isAsc);
        case 'Surname':
          return compare(a.surname, b.surname, isAsc);
        case 'Login':
          return compare(a.login, b.login, isAsc);
        case 'Email':
          return compare(a.email, b.email, isAsc);
        case 'Role':
          return compare(a.roles[0], b.roles[0], isAsc);
        default:
          return 0;
      }
    });

    /** Simple sort comparator for example ID/Name columns (for client-side sorting). */
    function compare(a: string | number, b: string | number, isAsc: boolean): number {
      return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

  }

  private setupFilter() {
    this.filterSubject.pipe(
      map(filterValue => {
        if (!filterValue.trim()) {
          return this.originalData;
        }
        return this.originalData.filter(user =>
          user.nickname.toLowerCase().includes(filterValue.toLowerCase()) ||
          user.name.toLowerCase().includes(filterValue.toLowerCase()) ||
          user.surname.toLowerCase().includes(filterValue.toLowerCase()) ||
          user.login.toLowerCase().includes(filterValue.toLowerCase()) ||
          user.email.toLowerCase().includes(filterValue.toLowerCase())
        );
      })
    ).subscribe(filteredData => {
      this.dataSubject.next(filteredData);
    });
  }

  filter(filterValue: string) {
    this.filterSubject.next(filterValue);
  }


  removeUsers(ids: number[]) {
    const currentData = this.dataSubject.value;
    const updatedData = currentData.filter(user => !ids.includes(user.id));
    this.originalData = updatedData;
    this.dataSubject.next(updatedData);
  }
}
