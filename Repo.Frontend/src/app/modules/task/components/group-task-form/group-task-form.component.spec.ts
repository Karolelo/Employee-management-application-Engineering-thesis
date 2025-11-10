import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupTaskFormComponent } from './group-task-form.component';

describe('GroupTaskFormComponent', () => {
  let component: GroupTaskFormComponent;
  let fixture: ComponentFixture<GroupTaskFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupTaskFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupTaskFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
