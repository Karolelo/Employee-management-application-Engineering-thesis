import { Component } from '@angular/core';
import { Location } from '@angular/common';
@Component({
  selector: 'app-not-found-page404',
  standalone: true,
  templateUrl: './not-found-page404.component.html',
  styleUrl: './not-found-page404.component.css'
})
export class NotFoundPage404Component {
  constructor(private location: Location) {}

  goBack(): void {
    this.location.back();
  }
}
