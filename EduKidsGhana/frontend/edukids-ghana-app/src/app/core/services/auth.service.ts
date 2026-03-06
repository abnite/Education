import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { ApiResponse, AuthResult, LoginRequest, RegisterParentRequest, UserInfo } from '../models/auth.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'ek_token';
  private readonly REFRESH_KEY = 'ek_refresh';
  private readonly USER_KEY = 'ek_user';

  currentUser = signal<UserInfo | null>(this.getStoredUser());
  isAuthenticated = signal<boolean>(!!this.getStoredToken());

  constructor(private http: HttpClient, private router: Router) {}

  login(request: LoginRequest): Observable<ApiResponse<AuthResult>> {
    return this.http.post<ApiResponse<AuthResult>>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap(res => { if (res.success && res.data) this.storeAuth(res.data); })
    );
  }

  registerParent(request: RegisterParentRequest): Observable<ApiResponse<AuthResult>> {
    return this.http.post<ApiResponse<AuthResult>>(`${environment.apiUrl}/auth/register/parent`, request).pipe(
      tap(res => { if (res.success && res.data) this.storeAuth(res.data); })
    );
  }

  logout(): void {
    const refreshToken = localStorage.getItem(this.REFRESH_KEY);
    if (refreshToken) {
      this.http.post(`${environment.apiUrl}/auth/logout`, { refreshToken }).subscribe();
    }
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null { return localStorage.getItem(this.TOKEN_KEY); }

  hasRole(role: string): boolean {
    const user = this.currentUser();
    return user?.roles?.includes(role) ?? false;
  }

  isAdmin(): boolean { return this.hasRole('Admin'); }
  isParent(): boolean { return this.hasRole('Parent'); }
  isLearner(): boolean { return this.hasRole('Learner'); }

  private storeAuth(auth: AuthResult): void {
    if (auth.token) localStorage.setItem(this.TOKEN_KEY, auth.token);
    if (auth.refreshToken) localStorage.setItem(this.REFRESH_KEY, auth.refreshToken);
    if (auth.user) {
      localStorage.setItem(this.USER_KEY, JSON.stringify(auth.user));
      this.currentUser.set(auth.user);
      this.isAuthenticated.set(true);
    }
  }

  private getStoredToken(): string | null { return localStorage.getItem(this.TOKEN_KEY); }
  private getStoredUser(): UserInfo | null {
    const stored = localStorage.getItem(this.USER_KEY);
    return stored ? JSON.parse(stored) : null;
  }
}
