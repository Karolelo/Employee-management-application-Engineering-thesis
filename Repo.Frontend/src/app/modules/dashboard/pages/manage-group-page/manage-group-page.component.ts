import { Component,inject } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import {map} from 'rxjs/operators';
@Component({
  selector: 'app-manage-group-page',
  standalone: false,
  templateUrl: './manage-group-page.component.html',
  styleUrl: './manage-group-page.component.css'
})
export class ManageGroupPageComponent {
  private breakpointObserver = inject(BreakpointObserver);

  cards = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map(({ matches }) => {
      if (matches) {
        return [
          { title: 'Card 1', cols: 1, rows: 1,type: 'announcement' },
          { title: 'Card 2', cols: 1, rows: 2,type: 'taskStats' },
          { title: 'Card 3', cols: 1, rows: 3,type: 'groupsStats' },
        ];
      }

      return [
        { title: 'announcements', cols: 2, rows: 1, type: 'announcement' },
        { title: 'Task management', cols: 2, rows: 2, type: 'taskStats' },
        { title: 'User management', cols: 1, rows: 2, type: 'groupStats'},
      ];
    })
  );
}
