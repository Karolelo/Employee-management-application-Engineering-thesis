import { Component } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
@Component({
  selector: 'app-side-navbar',
  standalone: false,
  templateUrl: './side-navbar.component.html',
  styleUrl: './side-navbar.component.css'
})
export class SideNavbarComponent {
    public isExpanded = false;

    public toggleMenu() {
      this.isExpanded = !this.isExpanded;
    }
}
