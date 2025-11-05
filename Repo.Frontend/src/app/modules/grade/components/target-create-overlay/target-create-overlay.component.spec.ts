import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetCreateOverlayComponent } from './target-create-overlay.component';

describe('TargetCreateOverlayComponent', () => {
  let component: TargetCreateOverlayComponent;
  let fixture: ComponentFixture<TargetCreateOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TargetCreateOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TargetCreateOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
