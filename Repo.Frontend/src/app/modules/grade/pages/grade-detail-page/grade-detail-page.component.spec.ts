import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradeDetailPageComponent } from './grade-detail-page.component';

describe('GradeDetailPageComponent', () => {
  let component: GradeDetailPageComponent;
  let fixture: ComponentFixture<GradeDetailPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GradeDetailPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GradeDetailPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
