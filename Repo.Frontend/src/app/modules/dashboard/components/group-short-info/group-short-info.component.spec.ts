import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupShortInfoComponent } from './group-short-info.component';

describe('GroupShortInfoComponent', () => {
  let component: GroupShortInfoComponent;
  let fixture: ComponentFixture<GroupShortInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupShortInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupShortInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
