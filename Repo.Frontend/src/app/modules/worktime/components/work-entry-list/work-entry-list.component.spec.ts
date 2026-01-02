import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkEntryListComponent } from './work-entry-list.component';

describe('WorkEntryListComponent', () => {
  let component: WorkEntryListComponent;
  let fixture: ComponentFixture<WorkEntryListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WorkEntryListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkEntryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
