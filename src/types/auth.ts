export interface LoginCredentials {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterCredentials {
  name: string;
  email: string;
  password: string;
  societyName: string;
  societyAddress: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  roles: string[];
  accessTokenExpiresAt?: string;
  refreshTokenExpiresAt?: string;
  societyId?: number;
  userId?: number;
}

export interface User {
  id: string;
  email: string;
  name: string;
  userName?: string;
  role?: string;
  roles: string[];
  societyId: string;
  societyName?: string | null;
  forcePasswordChange?: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface AuthState {
  user: User | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}
