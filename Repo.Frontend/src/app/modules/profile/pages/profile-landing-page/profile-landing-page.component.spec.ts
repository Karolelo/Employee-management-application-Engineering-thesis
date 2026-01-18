import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileLandingPageComponent } from './profile-landing-page.component';

describe('ProfileLandingPageComponent', () => {
  let component: ProfileLandingPageComponent;
  let fixture: ComponentFixture<ProfileLandingPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProfileLandingPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfileLandingPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
