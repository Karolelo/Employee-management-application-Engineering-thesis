import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {GradePageComponent} from './pages/grade-page/grade-page.component';
import {GradeDetailPageComponent} from './pages/grade-detail-page/grade-detail-page.component';

const routes: Routes = [
  {path: '', component: GradePageComponent},
  {path: 'grade-details/:id', component: GradeDetailPageComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GradeRoutingModule {
}
