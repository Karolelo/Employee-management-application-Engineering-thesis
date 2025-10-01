import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TasksDetailPageComponent } from './tasks-detail-page.component';

describe('TasksDetailPageComponent', () => {
  let component: TasksDetailPageComponent;
  let fixture: ComponentFixture<TasksDetailPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TasksDetailPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TasksDetailPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
