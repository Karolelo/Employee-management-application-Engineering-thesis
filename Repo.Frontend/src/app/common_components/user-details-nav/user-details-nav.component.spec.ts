import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserDetailsNavComponent } from './user-details-nav.component';

describe('UserDetailsNavComponent', () => {
  let component: UserDetailsNavComponent;
  let fixture: ComponentFixture<UserDetailsNavComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UserDetailsNavComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserDetailsNavComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
