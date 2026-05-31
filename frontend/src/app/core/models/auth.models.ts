export type UserRole = 'handler' | 'supervisor' | 'manager';

export interface MockUser {
  username: string;
  displayName: string;
  role: UserRole;
}

export interface LoginRequest {
  username: string;
  password?: string;
}

export interface LoginResponse {
  token: string;
  role: string;
  userName: string;
}

export interface AuthState {
  token: string | null;
  role: UserRole | null;
  userName: string | null;
}
