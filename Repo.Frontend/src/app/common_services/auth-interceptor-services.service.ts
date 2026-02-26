import {BehaviorSubject, catchError, filter, finalize, Observable, switchMap, take, throwError} from 'rxjs';
import {HttpErrorResponse, HttpEvent, HttpHeaders, HttpRequest} from '@angular/common/http';
import {HttpHandler, HttpInterceptor} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {AuthService} from '../modules/login/services/auth_managment/auth.service';

@Injectable()
export class AuthInterceptorService implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    //If endpoint is public we not adding token, especially during login
    if (this.isPublicEndpoint(request.url)) {
      return next.handle(request);
    }

    const token = this.authService.getToken();
    if (token) {
      request = this.addTokenHeader(request, token);
    }

    return next.handle(request).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse) {

          if (error.status === 401) {
            return this.handle401Error(request, next);
          }
          else if (error.status === 403) {
            // Manage not access
            return throwError(() => new Error('No access'));
          }
        }

        return throwError(() => error);
      })
    );
  }

  private isPublicEndpoint(url: string): boolean {
    const publicUrls = [
      '/api/auth/login',
      '/api/auth/register',
      '/api/auth/refresh-token'
    ];
    return publicUrls.some(publicUrl => url.includes(publicUrl));
  }
  private addTokenHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
    // Check if request is form
    const isFormData = request.body instanceof FormData;

    // In all other cases just push it as regular application/json
    const headers = isFormData
      ? new HttpHeaders({ 'Authorization': `Bearer ${token}` })
      : new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

    return request.clone({ headers });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    console.log('handle401Error');
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response) => {
          this.isRefreshing = false;
          const token = response.token || response;
          this.refreshTokenSubject.next(token);

          return next.handle(this.addTokenHeader(request, token.accessToken));
        }),
        catchError((error) => {
          this.isRefreshing = false;
          this.authService.logout();

          return throwError(() => new Error('Session expired. Please log in again'));
        }),
        finalize(() => {
          this.isRefreshing = false;
        })
      );
    }

    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap(token => {
        return next.handle(this.addTokenHeader(request, token.accessToken));
      }),
      catchError((error) => {
        this.authService.logout();
        return throwError(() => error);
      })
    );
  }
}
