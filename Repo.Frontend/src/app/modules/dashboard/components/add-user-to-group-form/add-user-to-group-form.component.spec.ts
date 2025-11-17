import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUserToGroupFormComponent } from './add-user-to-group-form.component';

describe('AddUserToGroupFormComponent', () => {
  let component: AddUserToGroupFormComponent;
  let fixture: ComponentFixture<AddUserToGroupFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddUserToGroupFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddUserToGroupFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
