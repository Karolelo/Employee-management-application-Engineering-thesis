import {Component, inject, OnInit} from '@angular/core';
import {MatIconModule} from '@angular/material/icon'
import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {BehaviorSubject, filter, map} from 'rxjs';
@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  moduleName: string = "Basic module";

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    this.initNavbar()
  }
  initNavbar(){
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => {
        let route: ActivatedRoute | null = this.activatedRoute;
        let moduleTitle = '';

        while (route !== null) {
          if (route.snapshot.data['title']) {
            moduleTitle = route.snapshot.data['title'];
          }
          route = route.firstChild;
        }
        return moduleTitle;
      })
    ).subscribe(title => {
      this.moduleName = title || "Basic module";
    });
  }

}
