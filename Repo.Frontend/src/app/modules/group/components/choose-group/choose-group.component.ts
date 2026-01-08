import { Component, inject, OnInit } from '@angular/core';
import {GroupService} from '../../../dashboard/services/group/group.service';
import {Group} from '../../../dashboard/interfaces/group';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {firstValueFrom} from 'rxjs'
import {Router,ActivatedRoute} from '@angular/router'
@Component({
  selector: 'app-choose-group',
  standalone: false,
  templateUrl: './choose-group.component.html',
  styleUrl: './choose-group.component.css'
})
export class ChooseGroupComponent implements OnInit{
  group_service: GroupService = inject(GroupService)
  user_store_service: UserStoreService = inject(UserStoreService)
  groups: Group[] = [];
  errorMessage: string = '';
  selectedGroupId: number | null = null;
  router: Router = inject(Router)
  activatedRoute: ActivatedRoute = inject(ActivatedRoute)
  async ngOnInit(){
    const userId = this.user_store_service.getUserId();
    //tbh im almost sure that this id is never empty soo
    //I'm not implementing else
    if(userId) {
      this.groups = await firstValueFrom(this.group_service.getUserGroups(userId))
    }
  }

  onButtonClick(){
    if(this.selectedGroupId){
      //Here we navigate relatively to path, quiet good option sometimes if u need to make
      //modules more indepentend from main navigation
      this.router.navigate(['view', this.selectedGroupId], { relativeTo: this.activatedRoute });
    }else {
      this.errorMessage = 'Please select group you want to see !!'
    }
  }
}
