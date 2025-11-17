import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, Subject } from 'rxjs';
import { GroupFormComponent } from './group-form.component';
import { GroupService } from '../../services/group/group.service';
import { UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';
import { By } from '@angular/platform-browser';
import { NO_ERRORS_SCHEMA } from '@angular/core';

class GroupServiceMock {
  createGroup = jasmine.createSpy('createGroup').and.returnValue(of({ id: 123 }));
  updateGroup = jasmine.createSpy('updateGroup').and.returnValue(of({ id: 1 }));
}
class UserServiceMock {
  private subj = new Subject<any[]>();
  getTeamLeadsWithoutGroup() { return this.subj.asObservable(); }
  emitUsers(users: any[]) { this.subj.next(users); }
}
class RouterStub { navigate = jasmine.createSpy('navigate'); }

describe('GroupFormComponent (edit/create behaviors)', () => {
  let component: GroupFormComponent;
  let fixture: ComponentFixture<GroupFormComponent>;
  let groupService: GroupServiceMock;
  let userService: UserServiceMock;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupFormComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: GroupService, useClass: GroupServiceMock },
        { provide: UserService, useClass: UserServiceMock },
        { provide: Router, useClass: RouterStub },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GroupFormComponent);
    component = fixture.componentInstance;
    groupService = TestBed.inject(GroupService) as unknown as GroupServiceMock;
    userService = TestBed.inject(UserService) as unknown as UserServiceMock;
  });

  it('disables admin_ID control when allowAdminEdit = false', () => {
    component.allowAdminEdit = false;
    fixture.detectChanges(); // triggers ngOnChanges once implemented
    const ctrl = component.groupForm.get('admin_ID');
    expect(ctrl?.disabled).toBeTrue(); // should be disabled for non-admins
  });

  it('enables admin_ID control when allowAdminEdit = true', () => {
    component.allowAdminEdit = true;
    fixture.detectChanges();
    const ctrl = component.groupForm.get('admin_ID');
    expect(ctrl?.enabled).toBeTrue();
  });

  it('patches form with input group in edit mode and shows Update group on button', () => {
    const longDesc = 'x'.repeat(40);
    component.group = { id: 1, name: 'G1', description: longDesc, admin_ID: 7 } as any;
    fixture.detectChanges();

    expect(component.groupForm.value).toEqual(jasmine.objectContaining({
      name: 'G1',
      description: longDesc,
      admin_ID: 7
    }));

    const btn = fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement as HTMLButtonElement;
    expect(btn.textContent?.toLowerCase()).toContain('update');
  });

  it('calls updateGroup on submit when editing (group provided)', () => {
    // Arrange edit mode
    const longDesc = 'y'.repeat(40);
    component.group = { id: 1, name: 'Group X', description: longDesc, admin_ID: 5 } as any;
    fixture.detectChanges();

    // Change a field to simulate user edit
    component.groupForm.get('name')?.setValue('Group X edited');

    // Act
    component.onSubmit();

    // Assert
    expect(groupService.updateGroup).toHaveBeenCalled();
    expect(groupService.createGroup).not.toHaveBeenCalled();
  });
});
