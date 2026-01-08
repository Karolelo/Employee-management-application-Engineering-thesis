import { TestBed } from '@angular/core/testing';

import { WorktimeUserService } from './worktime-user.service';

describe('WorktimeUserService', () => {
  let service: WorktimeUserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WorktimeUserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
