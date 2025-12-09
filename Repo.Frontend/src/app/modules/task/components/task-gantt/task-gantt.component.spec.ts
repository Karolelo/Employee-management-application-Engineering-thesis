import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskGanttComponent } from './task-gantt.component';

describe('TaskGanttComponent', () => {
  let component: TaskGanttComponent;
  let fixture: ComponentFixture<TaskGanttComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TaskGanttComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TaskGanttComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
