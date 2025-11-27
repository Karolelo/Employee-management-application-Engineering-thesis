import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GroupRoutingModule } from './group-routing.module';
import { GroupViewPageComponent } from './pages/group-view-page/group-view-page.component';
import { ChooseGroupComponent } from './components/choose-group/choose-group.component';
import { DashboardModule } from '../dashboard/dashboard.module';
import { InfoMessageComponent } from '../../common_components/info-message/info-message.component';
import { TaskModule } from '../task/task.module';
import { MatDivider } from '@angular/material/divider';
import { MatSelect,MatOption } from '@angular/material/select'
import { MatButton} from '@angular/material/button'
@NgModule({
  declarations: [
    GroupViewPageComponent,
    ChooseGroupComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    GroupRoutingModule,
    DashboardModule,
    InfoMessageComponent,
    TaskModule,
    MatDivider,
    MatSelect,
    MatOption,
    MatButton
  ]
})
export class GroupModule { }
