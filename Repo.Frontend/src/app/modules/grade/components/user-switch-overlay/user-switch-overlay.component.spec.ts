import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserSwitchOverlayComponent } from './user-switch-overlay.component';

describe('UserSwitchOverlayComponent', () => {
  let component: UserSwitchOverlayComponent;
  let fixture: ComponentFixture<UserSwitchOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UserSwitchOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserSwitchOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
