import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskGanttPageComponent } from './task-gantt-page.component';

describe('TaskGanttPageComponent', () => {
  let component: TaskGanttPageComponent;
  let fixture: ComponentFixture<TaskGanttPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TaskGanttPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TaskGanttPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
