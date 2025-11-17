import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupTaskListComponent } from './group-task-list.component';

describe('GroupTaskListComponent', () => {
  let component: GroupTaskListComponent;
  let fixture: ComponentFixture<GroupTaskListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupTaskListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupTaskListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
