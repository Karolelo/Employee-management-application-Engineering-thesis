import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { GroupImageEditFormComponent } from './group-image-edit-form.component';
import { GroupService } from '../../services/group/group.service';
import { FormBuilder } from '@angular/forms';
import { NO_ERRORS_SCHEMA } from '@angular/core';

class GroupServiceMock {
  getGroupImagePath = jasmine.createSpy('getGroupImagePath');
  saveGroupImage = jasmine.createSpy('saveGroupImage');
}

describe('GroupImageEditFormComponent', () => {
  let component: GroupImageEditFormComponent;
  let fixture: ComponentFixture<GroupImageEditFormComponent>;
  let service: GroupServiceMock;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      declarations: [GroupImageEditFormComponent],
      providers: [
        FormBuilder,
        { provide: GroupService, useClass: GroupServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(GroupImageEditFormComponent);
    component = fixture.componentInstance;
    service = TestBed.inject(GroupService) as unknown as GroupServiceMock;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should use default image when none exists (404 from service)', () => {
    const group = { id: 1 } as any;
    service.getGroupImagePath.and.returnValue(throwError(() => ({ status: 404 })));

    component.group = group;
    component.ngOnChanges({ group: { currentValue: group, previousValue: undefined, firstChange: true, isFirstChange: () => true } });

    expect(service.getGroupImagePath).toHaveBeenCalledWith(1);
    // imageUrl should remain undefined to force template fallback to default
    expect(component.imageUrl).toBeUndefined();
  });

  it('should call getGroupImagePath when group input changes', () => {
    const group = { id: 2 } as any;
    const blob = new Blob(['test'], { type: 'image/png' });
    service.getGroupImagePath.and.returnValue(of(blob));

    component.group = group;
    component.ngOnChanges({ group: { currentValue: group, previousValue: undefined, firstChange: true, isFirstChange: () => true } });

    expect(service.getGroupImagePath).toHaveBeenCalledWith(2);
    expect(component.imageUrl).toBeTruthy();
  });

  it('onFileSent should call saveGroupImage with isUpdate=false when no current image', fakeAsync(() => {
    const file = new File(['abc'], 'x.png');
    component.group = { id: 10 } as any;
    service.saveGroupImage.and.returnValue(of({}));

    component.imageUrl = undefined; // no image yet
    const spyInit = spyOn<any>(component, 'initializeValues').and.callFake(() => {});

    component.onFileSent(file);

    expect(service.saveGroupImage).toHaveBeenCalledWith(10, file, false);
    tick(3000);
    expect(spyInit).toHaveBeenCalledTimes(1);
  }));

  it('onFileSent should call saveGroupImage with isUpdate=true when image exists', fakeAsync(() => {
    const file = new File(['abc'], 'x.png');
    component.group = { id: 11 } as any;
    service.saveGroupImage.and.returnValue(of({}));
    component.imageUrl = 'safeurl' as any; // some image exists
    const spyInit = spyOn<any>(component, 'initializeValues').and.callFake(() => {});

    component.onFileSent(file);

    expect(service.saveGroupImage).toHaveBeenCalledWith(11, file, true);
    tick(3000);
    expect(spyInit).toHaveBeenCalledTimes(1);
  }));
});
