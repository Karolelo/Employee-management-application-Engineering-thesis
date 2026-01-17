import {RouterModule, Routes} from '@angular/router';
import {NgModule,inject} from '@angular/core';
import {GroupViewPageComponent} from './pages/group-view-page/group-view-page.component';
import {ChooseGroupComponent} from './components/choose-group/choose-group.component';
import {groupGuard} from './guards/group.guard';

const routes: Routes = [
  {
    path: 'view/:id', component: GroupViewPageComponent,
    canActivate: [groupGuard]
  },
  {
    path: '', component: ChooseGroupComponent
  }
]
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GroupRoutingModule { }
