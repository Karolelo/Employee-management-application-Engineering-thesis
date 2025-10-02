import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {GradePageComponent} from './pages/grade-page/grade-page.component';

const routes: Routes = [
  {path: '', component: GradePageComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GradeRoutingModule {
}
