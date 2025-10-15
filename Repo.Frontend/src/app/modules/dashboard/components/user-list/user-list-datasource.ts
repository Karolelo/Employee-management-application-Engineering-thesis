import { DataSource } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { map } from 'rxjs/operators';
import { Observable, of as observableOf, merge } from 'rxjs';
import {User} from '../../interfaces/user';
import { BehaviorSubject,finalize,combineLatest } from 'rxjs';
import {UserService} from '../../services/user.service';

/**
 * Data source for the UserList view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
export class UserListDataSource extends DataSource<User> {
  dataSubject: BehaviorSubject<User[]> = new BehaviorSubject<User[]>([]);
  loadingSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();
  filterSubject: BehaviorSubject<string> = new BehaviorSubject<string>('');
  paginator: MatPaginator | undefined;
  sort: MatSort | undefined;
  constructor(private user_service: UserService) {
    super();
    this.loadUser();
  }

  loadUser(){
    this.loadingSubject.next(true);

    this.user_service.getUsers().pipe(
      finalize(() => this.loadingSubject.next(false)),
    ).subscribe(
      (users) => {
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
  //Need to future implement this shit
  /*deleteUser(id: number) {
    this.data = this.data.filter(x => x.id !== id);
  }*/

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
        /*TODO zastanowić się jak to filtrować
        case 'Role':
          return compare(a.role, b.role, isAsc);*/
        default:
          return 0;
      }
    });

    /** Simple sort comparator for example ID/Name columns (for client-side sorting). */
    function compare(a: string | number, b: string | number, isAsc: boolean): number {
      return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

  }
  filter(filterValue: string){
   /* this.data = EXAMPLE_DATA.filter(item => {
      return (
        item.nickname.toLowerCase().includes(filterValue) ||
        item.name.toLowerCase().includes(filterValue) ||
        item.surname.toLowerCase().includes(filterValue) ||
        item.login.toLowerCase().includes(filterValue) ||
        item.email.toLowerCase().includes(filterValue) ||
        item.role.toLowerCase().includes(filterValue)
      );
    });*/
    this.filterSubject.next(filterValue);
    combineLatest([
      this.dataSubject,
      this.filterSubject
    ]).pipe(
      map(([users, filterValue]) =>
        users.filter(user =>
          user.nickname.toLowerCase().includes(filterValue.toLowerCase()) ||
          user.name.toLowerCase().includes(filterValue) ||
          user.surname.toLowerCase().includes(filterValue) ||
          user.login.toLowerCase().includes(filterValue) ||
          user.email.toLowerCase().includes(filterValue)
        )
      )).subscribe(
        filteredUsers => {
          this.dataSubject.next(filteredUsers);
        }
    )
  }

  removeUsers(ids: number[]) {
    const currentData = this.dataSubject.value;
    const updatedData = currentData.filter(user => !ids.includes(user.id));
    this.dataSubject.next(updatedData);
  }
}
