import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateUserStepperFormComponent } from './create-user-stepper-form.component';

describe('CreateUserStepperFormComponent', () => {
  let component: CreateUserStepperFormComponent;
  let fixture: ComponentFixture<CreateUserStepperFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateUserStepperFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateUserStepperFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
