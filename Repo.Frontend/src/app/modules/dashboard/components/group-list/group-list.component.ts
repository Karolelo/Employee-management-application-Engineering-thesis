import { Component,Input,Output,EventEmitter } from '@angular/core';
import {Group} from '../../interfaces/group';
import { Router } from '@angular/router';
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
  @Input() groups: Group[] = [];
  @Output() groupDeleteEvent: EventEmitter<number> = new EventEmitter<number>();
  currentPage: number;
  itemsPerPage: number;
  constructor(private router: Router) {
    this.currentPage = 1;
    this.itemsPerPage = 18;
  }

  onEdiGroup(id: number) {

  }

  onDeleteGroup(id: number) {
    this.groupDeleteEvent.emit(id);
  }

  onShowGroup(id: number) {
    this.router.navigate(['/dashboard/groups', id]);
  }
}
