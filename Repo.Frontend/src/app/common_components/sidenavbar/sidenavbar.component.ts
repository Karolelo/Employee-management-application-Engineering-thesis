import { Component,inject,OnInit } from '@angular/core';
import {UserStoreService} from '../../modules/login/services/user_data/user-store.service';

@Component({
    selector: 'app-sidenavbar',
    standalone: false,
    templateUrl: './sidenavbar.component.html',
    styleUrl: './sidenavbar.component.css'
})
export class SidenavbarComponent implements OnInit {
    private readonly userStoreService = inject(UserStoreService);

    private readonly PRIVILEGED_ROLES = ['Admin', 'TeamLeader'] as const;

    private readonly DASHBOARD_MENU_ITEM: MenuItem = {link: '/dashboard', icon: 'dashboard'};

    private readonly BASE_MENU_ITEMS: readonly MenuItem[] = [
        {link: '/tasks', icon: 'assignment'},
        {link: '/tasks/gantt', icon: 'view_timeline'},
        {link: '/calendar', icon: 'today'},
        {link: '/grades', icon: 'analytics'},
        {link: '/worktime', icon: 'next_week'},
        {link: '/myGroups', icon: 'group'}
    ];

    menuItems: MenuItem[] = [...this.BASE_MENU_ITEMS];

    ngOnInit(): void {
        if (!this.hasAnyRole(this.PRIVILEGED_ROLES)) return;

        this.menuItems.push(this.DASHBOARD_MENU_ITEM);
    }

    private hasAnyRole(roles: readonly string[]): boolean {
        return roles.some(role => this.userStoreService.hasRole(role));
    }
}

type MenuItem = {
    link: string;
    icon: string;
};
