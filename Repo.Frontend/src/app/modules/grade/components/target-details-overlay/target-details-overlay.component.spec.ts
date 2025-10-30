import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetDetailsOverlayComponent } from './target-details-overlay.component';

describe('TargetDetailsOverlayComponent', () => {
  let component: TargetDetailsOverlayComponent;
  let fixture: ComponentFixture<TargetDetailsOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TargetDetailsOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TargetDetailsOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
