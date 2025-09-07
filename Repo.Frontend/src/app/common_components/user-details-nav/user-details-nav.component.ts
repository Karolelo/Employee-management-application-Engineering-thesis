import {Component, ViewChild} from '@angular/core';
import {MatSidenav} from '@angular/material/sidenav';
import {AuthService} from '../../modules/login/services/auth_managment/auth.service';

@Component({
  selector: 'app-user-details-nav',
  standalone: false,
  templateUrl: './user-details-nav.component.html',
  styleUrl: './user-details-nav.component.css'
})
export class UserDetailsNavComponent {

  constructor(private authService: AuthService,){
  }

  logout() {
    this.authService.logout();
  }
}
