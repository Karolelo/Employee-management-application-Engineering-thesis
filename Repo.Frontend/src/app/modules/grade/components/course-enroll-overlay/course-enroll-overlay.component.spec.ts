import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseEnrollOverlayComponent } from './course-enroll-overlay.component';

describe('CourseEnrollOverlayComponent', () => {
  let component: CourseEnrollOverlayComponent;
  let fixture: ComponentFixture<CourseEnrollOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CourseEnrollOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CourseEnrollOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
