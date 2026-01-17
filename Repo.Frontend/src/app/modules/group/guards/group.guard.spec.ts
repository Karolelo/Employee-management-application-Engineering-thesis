import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { groupGuard } from './group.guard';

describe('groupGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => groupGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
