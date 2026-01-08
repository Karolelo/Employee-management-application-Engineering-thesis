import {Component, EventEmitter, OnDestroy, OnInit, Output} from '@angular/core';
import {FormControl} from '@angular/forms';
import {UserMini} from '../../interfaces/user-mini';
import {debounceTime, distinctUntilChanged, Subscription, switchMap} from 'rxjs';
import {UserService} from '../../services/user.service';

@Component({
  selector: 'app-user-switch-overlay',
  standalone: false,
  templateUrl: './user-switch-overlay.component.html',
  styleUrl: './user-switch-overlay.component.css'
})
export class UserSwitchOverlayComponent implements OnInit, OnDestroy {
  @Output() close = new EventEmitter<void>();
  @Output() selected = new EventEmitter<{ id: number; name: string }>();

  query = new FormControl<string>('', {nonNullable: true});
  users: UserMini[] = [];
  loading = true;
  sub?: Subscription;

  constructor(private userService: UserService) {
  }

  ngOnInit(): void {
    this.sub = this.query.valueChanges.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(query => this.userService.getUsers(query ?? '', 1, 20))
    ).subscribe(list => {
      this.users = list;
      this.loading = false;
    });

    this.query.setValue('');
  }

  ngOnDestroy(): void {this.sub?.unsubscribe();}

  clear(): void {this.query.setValue('');}

  pick(user: UserMini) {
    const name = user.nickname?.trim() || user.login;
    this.selected.emit({id: user.id, name: name});
  }

  submitId(): void {
    const raw = this.query.value?.trim() || '';
    const id = Number(raw);
    if (!id) return;
    this.loading = true;
    this.userService.getUserById(id).subscribe(user => {
      this.loading = false;
      if (!user) return;
      this.pick(user);
    });
  }

  displayName(user: UserMini): string {
    return user.nickname?.trim() || user.login || `ID: ${user.id}`;
  }
}
