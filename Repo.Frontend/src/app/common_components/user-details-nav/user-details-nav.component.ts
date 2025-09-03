import {Component, ViewChild} from '@angular/core';
import {MatSidenav} from '@angular/material/sidenav';

@Component({
  selector: 'app-user-details-nav',
  standalone: false,
  templateUrl: './user-details-nav.component.html',
  styleUrl: './user-details-nav.component.css'
})
export class UserDetailsNavComponent {
  /*@ViewChild('userDetailsNav') userDetailsNav!: MatSidenav;*/

  logout() {

  }
}
