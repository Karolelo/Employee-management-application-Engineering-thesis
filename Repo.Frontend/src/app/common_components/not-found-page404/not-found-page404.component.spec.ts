import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotFoundPage404Component } from './not-found-page404.component';

describe('NotFoundPage404Component', () => {
  let component: NotFoundPage404Component;
  let fixture: ComponentFixture<NotFoundPage404Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [NotFoundPage404Component]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotFoundPage404Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
