import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradeDetailsOverlayComponent } from './grade-details-overlay.component';

describe('GradeDetailsOverlayComponent', () => {
  let component: GradeDetailsOverlayComponent;
  let fixture: ComponentFixture<GradeDetailsOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GradeDetailsOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GradeDetailsOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
