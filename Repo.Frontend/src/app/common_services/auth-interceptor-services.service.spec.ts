import { TestBed } from '@angular/core/testing';

import { AuthInterceptorServicesService } from './auth-interceptor-services.service';

describe('AuthInterceptorServicesService', () => {
  let service: AuthInterceptorServicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthInterceptorServicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
