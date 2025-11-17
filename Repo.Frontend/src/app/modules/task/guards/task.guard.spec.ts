import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { taskGuard } from './task.guard';

describe('taskGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => taskGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
