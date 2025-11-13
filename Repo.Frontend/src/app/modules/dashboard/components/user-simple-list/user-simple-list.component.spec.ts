import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserSimpleListComponent } from './user-simple-list.component';

describe('UserSimpleListComponent', () => {
  let component: UserSimpleListComponent;
  let fixture: ComponentFixture<UserSimpleListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UserSimpleListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserSimpleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
