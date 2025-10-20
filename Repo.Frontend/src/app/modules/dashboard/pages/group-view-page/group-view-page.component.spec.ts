import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupViewPageComponent } from './group-view-page.component';

describe('GroupViewPageComponent', () => {
  let component: GroupViewPageComponent;
  let fixture: ComponentFixture<GroupViewPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupViewPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupViewPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
