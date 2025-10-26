import { Component, OnInit, ViewChild } from '@angular/core';
import {Grade} from '../../interfaces/grade';
import {ActivatedRoute, Router} from '@angular/router';
import {GradeService} from '../../services/grade.service';
import {take} from 'rxjs/operators';
import {TargetService} from '../../services/target.service';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-grade-page',
  standalone: false,
  templateUrl: './grade-page.component.html',
  styleUrl: './grade-page.component.css'
})
export class GradePageComponent implements OnInit {
  selected?: Grade;
  onEdit(grade: Grade) {this.selected = grade;}
  onChanged() {this.selected = undefined;}
  targetCount = 0;
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gradeService: GradeService,
    private targetService: TargetService,
    private userStore: UserStoreService
  ) {}
  ngOnInit(): void {
    this.route.queryParamMap.pipe(take(1)).subscribe(params => {
      const editId = Number(params.get('editId'));
      if (editId) {
        this.gradeService.getGradeById(editId).pipe(take(1)).subscribe({
          next: g => this.selected = g
        });
      }
    });
    this.targetService.targets$.subscribe(list => this.targetCount = list.length);
  }
  onChanged() {
    this.selected = undefined;
    this.router.navigate(['/grades'], { queryParams: {} });
  }
}
