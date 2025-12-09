import { Component,Input,Output,EventEmitter,inject } from '@angular/core';
import {Group} from '../../interfaces/group';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar'
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
@Component({
  selector: 'app-group-list',
  standalone: false,
  templateUrl: './group-list.component.html',
  styleUrl: './group-list.component.css'
})
//This component is more basic than task list
//because during development we thought that
//it will be easier to manage this component
//when it have less dependency
export class GroupListComponent {
  private readonly ADMIN_ROLE = 'Admin';
  private readonly NOT_HAVE_ADMIN_ROLE = "You don't have admin role, you can only see other the groups"

  @Input() groups: Group[] = [];
  @Output() groupDeleteEvent: EventEmitter<number> = new EventEmitter<number>();

  currentPage: number = 1;
  itemsPerPage: number = 10;
  router: Router = inject(Router)
  matSnackBar: MatSnackBar = inject(MatSnackBar)
  user_store_service: UserStoreService = inject(UserStoreService)
  constructor() {
  }

  onEditGroup(id: number) {
    if(this.canNavigate())
    this.router.navigate(['/dashboard/group/edit/',id])
  }

  onDeleteGroup(id: number) {
    if(this.canNavigate())
    this.groupDeleteEvent.emit(id);
  }

  onShowGroup(id: number) {
    this.router.navigate(['/myGroups/view/', id]);
  }

  private canNavigate(){
    if(this.user_store_service.hasRole(this.ADMIN_ROLE))
    {
      return true
    }
    else
    {
      this.showSnackBarMessage(this.NOT_HAVE_ADMIN_ROLE)
    }
    return false
  }
  private showSnackBarMessage(message: string, action: string = 'Ok', duration: number = 3000): void {
    this.matSnackBar.open(message, action, { duration });
  }

}
