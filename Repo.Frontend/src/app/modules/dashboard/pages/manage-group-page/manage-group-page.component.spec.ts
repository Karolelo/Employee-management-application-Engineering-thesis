import { Component, Input } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ManageGroupPageComponent } from './manage-group-page.component';
import { GroupService } from '../../services/group/group.service';
import { UserStoreService } from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-group-form',
  template: ''
})
class DummyGroupFormComponent {
  @Input() group: any;
  @Input() allowAdminEdit = true;
  @Input() transparentBackground = false;
}

class ActivatedRouteStub {
  snapshot = { paramMap: new Map([['id', '1']]) } as any;
}

class BreakpointObserverStub {
  observe() { return of({ matches: false }); }
}

class GroupServiceStub {
  getGroup() { return of({ id: 1, name: 'G', description: 'x'.repeat(40), admin_ID: 2 }); }
}

class UserStoreServiceStub {
  hasRole(role: string) { return role === 'Admin' ? true : false; }
}

describe('ManageGroupPageComponent integration', () => {
  let fixture: ComponentFixture<ManageGroupPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ManageGroupPageComponent, DummyGroupFormComponent],
      providers: [
        { provide: ActivatedRoute, useClass: ActivatedRouteStub },
        { provide: BreakpointObserver, useClass: BreakpointObserverStub },
        { provide: GroupService, useClass: GroupServiceStub },
        { provide: UserStoreService, useClass: UserStoreServiceStub },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ManageGroupPageComponent);
    fixture.detectChanges();
  });

  it('passes group and allowAdminEdit based on Admin role, and sets transparent background', () => {
    fixture.detectChanges();
    const groupFormDebug = fixture.debugElement.query((d) => d.name === 'app-group-form');
    expect(groupFormDebug).withContext('group form rendered').toBeTruthy();

    const instance = groupFormDebug.componentInstance as DummyGroupFormComponent;
    expect(instance.group).toEqual(jasmine.objectContaining({ id: 1 }));
    expect(instance.allowAdminEdit).toBeTrue();
    expect(instance.transparentBackground).toBeTrue();
  });
});
