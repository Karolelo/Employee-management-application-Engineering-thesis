import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupEditFormComponent } from './group-edit-form.component';

describe('GroupEditFormComponent', () => {
  let component: GroupEditFormComponent;
  let fixture: ComponentFixture<GroupEditFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupEditFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupEditFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
