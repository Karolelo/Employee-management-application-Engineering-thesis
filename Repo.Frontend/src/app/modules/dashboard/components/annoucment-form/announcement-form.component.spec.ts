import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnnouncementFormComponent } from './announcement-form.component';

describe('AnnoucmentFormComponent', () => {
  let component: AnnouncementFormComponent;
  let fixture: ComponentFixture<AnnouncementFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AnnouncementFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AnnouncementFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
