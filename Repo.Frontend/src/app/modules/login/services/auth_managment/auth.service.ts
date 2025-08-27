import {Inject, Injectable} from '@angular/core';
import {BehaviorSubject, catchError, Observable, tap, throwError} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {LoginRequest} from '../../interafaces/login-request';
import {TokenResponse} from '../../interafaces/token-response';
import {Router} from '@angular/router';
import {UserStoreService} from '../user_data/user-store.service';
import {JwtHelperService} from '@auth0/angular-jwt';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  constructor(private http: HttpClient,private router: Router,private storeService: UserStoreService,private jwtHelper: JwtHelperService) {
    this.checkInitialAuth();
  }

  private checkInitialAuth(): void {
    try {
      const token = localStorage.getItem(this.TOKEN_KEY);

      if (!token) {
        console.log('No token in local storage');
        this.isAuthenticatedSubject.next(false);
        return;
      }

      if (this.jwtHelper.isTokenExpired(token)) {
        console.log('tokenExpire');
        this.isAuthenticatedSubject.next(false);
        localStorage.removeItem(this.TOKEN_KEY);
        this.router.navigate(['/login']);
        return;
      }

      console.log('Old token still valid');
      this.isAuthenticatedSubject.next(true);
      this.storeService.storeUserData(token);
    } catch (error) {
      console.error('Error durring validation of token:', error);
      this.isAuthenticatedSubject.next(false);
      localStorage.removeItem(this.TOKEN_KEY);
      this.router.navigate(['/login']);
    }
  }

  login(creditals: LoginRequest): Observable<TokenResponse>
  {
    return this.http.post<TokenResponse>('/api/auth/login', creditals)
      .pipe(  tap(response => {
          console.log('login response '+response);
          this.storeTokens(response);
          this.isAuthenticatedSubject.next(true);
        }),
        catchError(error => {
          console.error('Error during login:', error);
          return throwError(() => error);
        })
      );
  }

  refreshToken(): Observable<TokenResponse> {
    const accessToken = this.getToken();
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);

    if (!accessToken || !refreshToken) {
      this.logout();
      return throwError(() => new Error('No authorization token or refresh token.'));
    }

    return this.http.post<TokenResponse>('/api/auth/refresh-token', {
      accessToken,
      refreshToken
    }).pipe(
      tap(response => {
        this.storeTokens(response);
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  private storeTokens(response: TokenResponse): void {
    localStorage.setItem(this.TOKEN_KEY,response.token.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY,response.token.refreshToken);
    this.storeService.storeUserData(response.token.accessToken);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isAuthenticated(): Observable<boolean> {
    return this.isAuthenticatedSubject.asObservable();
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/login']);
  }
}
