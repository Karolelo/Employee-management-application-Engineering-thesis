import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradeCreateOverlayComponent } from './grade-create-overlay.component';

describe('GradeCreateOverlayComponent', () => {
  let component: GradeCreateOverlayComponent;
  let fixture: ComponentFixture<GradeCreateOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GradeCreateOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GradeCreateOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
