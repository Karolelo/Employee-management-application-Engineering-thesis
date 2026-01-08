import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserGradeListComponent } from './user-grade-list.component';

describe('UserGradeListComponent', () => {
  let component: UserGradeListComponent;
  let fixture: ComponentFixture<UserGradeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UserGradeListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserGradeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
