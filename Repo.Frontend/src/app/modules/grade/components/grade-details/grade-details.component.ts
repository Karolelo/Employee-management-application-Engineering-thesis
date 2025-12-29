import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import { Grade } from '../../interfaces/grade';
import {GradeService} from '../../services/grade.service';

@Component({
  selector: 'app-grade-details',
  standalone: false,
  templateUrl: './grade-details.component.html',
  styleUrl: './grade-details.component.css'
})
export class GradeDetailsComponent implements OnInit {
  grade?: Grade;
  loading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private gradeService: GradeService
  ) {
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.gradeService.getGradeById(id).subscribe({
      next: grade => {
        this.grade = grade;
        this.loading = false;
        },
      error: () => {
        this.errorMessage = 'Failed to find grade';
        this.loading = false;
        }
    });
  }
}
