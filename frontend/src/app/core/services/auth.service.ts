import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, of } from 'rxjs';
import { environment } from '@env/environment';
import { AuthState, LoginResponse, MockUser, UserRole } from '../models/auth.models';

const TOKEN_KEY = 'claims_jwt_token';
const ROLE_KEY = 'claims_user_role';
const USERNAME_KEY = 'claims_user_name';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/auth`;

  readonly mockUsers: MockUser[] = [
    { username: 'handler', displayName: 'Claims Handler', role: 'handler' },
    { username: 'supervisor', displayName: 'Supervisor', role: 'supervisor' },
    { username: 'manager', displayName: 'Manager', role: 'manager' },
  ];

  private readonly state = signal<AuthState>(this.loadFromStorage());

  readonly token = computed(() => this.state().token);
  readonly role = computed(() => this.state().role);
  readonly userName = computed(() => this.state().userName);
  readonly isAuthenticated = computed(() => !!this.state().token);
  readonly canApproveReserves = computed(() => {
    const role = this.state().role;
    return role === 'supervisor' || role === 'manager';
  });

  constructor() {
    const stored = this.state();
    if (stored.token && !stored.role) {
      this.restoreSession(stored.token).subscribe();
    } else if (!stored.token) {
      this.switchUser(this.mockUsers[0]).subscribe();
    }
  }

  login(username: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.baseUrl}/login`, {
        username,
        password: 'demo',
      })
      .pipe(tap((res) => this.persistSession(res)));
  }

  switchUser(user: MockUser): Observable<LoginResponse> {
    return this.login(user.username);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ROLE_KEY);
    localStorage.removeItem(USERNAME_KEY);
    this.state.set({ token: null, role: null, userName: null });
  }

  getStoredToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private persistSession(res: LoginResponse): void {
    const role = res.role as UserRole;
    localStorage.setItem(TOKEN_KEY, res.token);
    localStorage.setItem(ROLE_KEY, role);
    localStorage.setItem(USERNAME_KEY, res.userName);
    this.state.set({ token: res.token, role, userName: res.userName });
  }

  private loadFromStorage(): AuthState {
    return {
      token: localStorage.getItem(TOKEN_KEY),
      role: (localStorage.getItem(ROLE_KEY) as UserRole) || null,
      userName: localStorage.getItem(USERNAME_KEY),
    };
  }

  private restoreSession(token: string): Observable<LoginResponse | null> {
    const user = this.mockUsers.find((u) => localStorage.getItem(ROLE_KEY) === u.role);
    if (user) {
      return this.login(user.username);
    }
    return of(null);
  }
}
