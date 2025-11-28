import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  documentId: string;
  phoneNumber: string;
  address: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  userId?: string;
  email?: string;
  userName?: string;
  roles?: string[];
  expiration?: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = 'http://localhost:5277/api/Auth';

  // Signals for reactive state
  isAuthenticated = signal(false);
  currentUser = signal<{ userId: string; email: string; userName: string; roles: string[] } | null>(null);

  constructor() {
    // Check if user is already logged in on service initialization
    this.checkAuthStatus();
  }

  private checkAuthStatus() {
    const token = this.getToken();
    if (token) {
      const user = this.getUserFromStorage();
      if (user) {
        this.isAuthenticated.set(true);
        this.currentUser.set(user);
      }
    }
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        if (response.success && response.token) {
          this.saveAuthData(response);
          this.isAuthenticated.set(true);
          this.currentUser.set({
            userId: response.userId!,
            email: response.email!,
            userName: response.userName!,
            roles: response.roles || []
          });
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    // Send the request directly - API expects firstName and lastName
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, request);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserId(): string | null {
    const user = this.getUserFromStorage();
    return user?.userId || null;
  }

  private saveAuthData(response: AuthResponse) {
    if (response.token) {
      localStorage.setItem('token', response.token);
    }
    if (response.userId && response.email && response.userName) {
      const user = {
        userId: response.userId,
        email: response.email,
        userName: response.userName,
        roles: response.roles || []
      };
      localStorage.setItem('user', JSON.stringify(user));
    }
  }

  private getUserFromStorage() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }
}
