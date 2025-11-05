import { Component, OnInit, ViewChild } from '@angular/core';
import {Grade} from '../../interfaces/grade';
import {ActivatedRoute, Router} from '@angular/router';
import {GradeService} from '../../services/grade.service';
import {take} from 'rxjs/operators';
import {TargetService} from '../../services/target.service';
import {CourseListComponent} from '../../components/course-list/course-list.component';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-grade-page',
  standalone: false,
  templateUrl: './grade-page.component.html',
  styleUrl: './grade-page.component.css'
})
export class GradePageComponent implements OnInit {
  selected?: Grade;
  selectedCourseId?: number;
  selectedTargetId?: number;
  selectedGradeId?: number;
  selectedUserId?: number;
  targetCount = 0;
  isLeaderOrAdmin = false;
  selectedUsername = '';
  userSwitchOpen = false;
  createCourseOpen = false;
  createTargetOpen = false;
  createGradeOpen = false;

  @ViewChild(CourseListComponent) courseList?: CourseListComponent;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gradeService: GradeService,
    private targetService: TargetService,
    private userStore: UserStoreService
  ) {}

  ngOnInit(): void {
    this.isLeaderOrAdmin = this.userStore.hasRole('TeamLeader') || this.userStore.hasRole('Admin');
    this.route.queryParamMap.pipe(take(1)).subscribe(params => {
      this.selectedUsername = params.get('selectedUser') || params.get('userName') || params.get('user') || '';
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

  onCourseSelect(courseId: number) {
    this.selectedCourseId = courseId;
  }
  onCourseOverlayClose(changed: boolean) {
    this.selectedCourseId = undefined;
    if (changed){
      this.courseList?.reload();
    }
  }

  onTargetSelect(targetId: number) {
    this.selectedTargetId = targetId;
  }
  onTargetOverlayClose(changed?: boolean) {
    this.selectedTargetId = undefined;
  }

  onGradeSelect(gradeId: number) {
    this.selectedGradeId = gradeId;
  }
  onGradeOverlayClose(changed?: boolean) {
    this.selectedGradeId = undefined;
  }

  onAddGrade() {
    this.createGradeOpen = true;
  }
  onGradeCreated(changed: boolean) {
    this.createGradeOpen = false;
  }

  onAddTarget() {
    this.createTargetOpen = true;
  }
  onTargetCreated(changed: boolean) {
    this.createTargetOpen = false;
  }

  onAddCourse() {
    this.createCourseOpen = true;
  }
  onCourseCreated(changed: boolean) {
    this.createCourseOpen = false;
    if (changed) this.courseList?.reload();
  }

  onHeaderMenu() {
    this.userSwitchOpen = true;
  }
  onUserSwitchClose() {
    this.userSwitchOpen = false;
  }
  onUserSelected(ev: {id: number; name: string}) {
    this.selectedUserId = ev.id;
    this.selectedUsername = ev.name;
    this.userSwitchOpen = false;
  }
}
