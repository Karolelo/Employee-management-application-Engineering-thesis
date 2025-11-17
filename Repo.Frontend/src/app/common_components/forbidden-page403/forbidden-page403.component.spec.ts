import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForbiddenPage403Component } from './forbidden-page403.component';

describe('ForbiddenPage403Component', () => {
  let component: ForbiddenPage403Component;
  let fixture: ComponentFixture<ForbiddenPage403Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ForbiddenPage403Component]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ForbiddenPage403Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
