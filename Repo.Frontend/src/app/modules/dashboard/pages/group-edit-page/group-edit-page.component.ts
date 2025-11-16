import { Component,OnInit } from '@angular/core';
import {Group} from '../../interfaces/group';
import {ActivatedRoute} from "@angular/router"

@Component({
  selector: 'app-group-edit-page',
  standalone: false,
  templateUrl: './group-edit-page.component.html',
  styleUrl: './group-edit-page.component.css'
})
export class GroupEditPageComponent implements OnInit{
  groupToEdit?: Group;
  constructor(private route: ActivatedRoute) {
  }

  ngOnInit(){
    this.groupToEdit = this.route.snapshot.data['group'];
  }
}
