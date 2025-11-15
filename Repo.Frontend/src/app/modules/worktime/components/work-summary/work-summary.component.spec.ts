import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkSummaryComponent } from './work-summary.component';

describe('WorkSummaryComponent', () => {
  let component: WorkSummaryComponent;
  let fixture: ComponentFixture<WorkSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WorkSummaryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
