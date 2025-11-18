import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WorktimePageComponent } from './pages/worktime-page/worktime-page.component';

const routes: Routes = [
  {
    path: '',
    component: WorktimePageComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WorktimeRoutingModule { }
