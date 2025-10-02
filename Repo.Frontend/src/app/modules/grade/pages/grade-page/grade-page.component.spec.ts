import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradePageComponent } from './grade-page.component';

describe('GradePageComponent', () => {
  let component: GradePageComponent;
  let fixture: ComponentFixture<GradePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GradePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GradePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
