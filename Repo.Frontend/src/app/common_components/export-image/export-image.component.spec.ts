import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportImageComponent } from './export-image.component';

describe('ExportImageComponent', () => {
  let component: ExportImageComponent;
  let fixture: ComponentFixture<ExportImageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExportImageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExportImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
