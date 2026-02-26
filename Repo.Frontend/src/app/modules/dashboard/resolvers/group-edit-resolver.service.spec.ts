import { TestBed } from '@angular/core/testing';

import { GroupEditResolverService } from './group-edit-resolver.service';

describe('GroupEditResolverService', () => {
  let service: GroupEditResolverService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GroupEditResolverService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
