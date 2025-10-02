import { Component } from '@angular/core';
import {Grade} from '../../interfaces/grade';

@Component({
  selector: 'app-grade-page',
  standalone: false,
  templateUrl: './grade-page.component.html',
  styleUrl: './grade-page.component.css'
})
export class GradePageComponent {
  selected?: Grade;
  onEdit(grade: Grade) {this.selected = grade;}
  onChanged() {this.selected = undefined;}
}
