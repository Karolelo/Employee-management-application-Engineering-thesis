import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseCreateOverlayComponent } from './course-create-overlay.component';

describe('CourseCreateOverlayComponent', () => {
  let component: CourseCreateOverlayComponent;
  let fixture: ComponentFixture<CourseCreateOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CourseCreateOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CourseCreateOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
