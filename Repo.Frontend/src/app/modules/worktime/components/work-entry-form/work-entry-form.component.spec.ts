import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkEntryFormComponent } from './work-entry-form.component';

describe('WorkEntryFormComponent', () => {
  let component: WorkEntryFormComponent;
  let fixture: ComponentFixture<WorkEntryFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WorkEntryFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkEntryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
