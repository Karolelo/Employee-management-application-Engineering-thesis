import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorktimePageComponent } from './worktime-page.component';

describe('WorktimePageComponent', () => {
  let component: WorktimePageComponent;
  let fixture: ComponentFixture<WorktimePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WorktimePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorktimePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
