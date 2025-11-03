import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupCreatePageComponent } from './group-create-page.component';

describe('GroupCreatePageComponent', () => {
  let component: GroupCreatePageComponent;
  let fixture: ComponentFixture<GroupCreatePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupCreatePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupCreatePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
