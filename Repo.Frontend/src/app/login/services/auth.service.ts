import { Injectable } from '@angular/core';
import {BehaviorSubject, catchError, Observable, tap, throwError} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {LoginRequest} from '../interafaces/login-request';
import {TokenResponse} from '../../common_models/token-response';
import {TokenModel} from '../interafaces/token-model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  constructor(private http: HttpClient) { }

  private checkInitialAuth():  void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    if (token) {
      this.isAuthenticatedSubject.next(true);
    }
  }

  login(creditals: LoginRequest): Observable<TokenResponse>
  {
    return this.http.post<TokenResponse>('/api/auth/login', creditals)
      .pipe(  tap(response => {
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
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);
    return this.http.post<TokenResponse>('/api/auth/refresh-token', {
      accessToken: this.getToken(),
      refreshToken: refreshToken
    }).pipe(
      tap(response => {
        this.storeTokens(response);
      })
    );
  }

  private storeTokens(response: TokenResponse): void {
    localStorage.setItem(this.TOKEN_KEY,response.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY,response.refreshToken);
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
  }
}
