import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetListComponent } from './target-list.component';

describe('TargetListComponent', () => {
  let component: TargetListComponent;
  let fixture: ComponentFixture<TargetListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TargetListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TargetListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
