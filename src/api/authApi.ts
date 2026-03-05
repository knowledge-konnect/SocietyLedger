import { LoginCredentials, RegisterCredentials, User, AuthResponse } from '../types/auth';
import { ApiResponse } from '../types/api';
import apiClient from './client';

/**
 * Converts AuthResponse to User object
 * Maps the login response data to User type
 * Backend response includes: accessToken, refreshToken, roles, userName, role, societyName, forcePasswordChange
 */
const authResponseToUser = (response: any): User => {
  console.log('Login response:', response); // Log to debug
  
  // Check multiple possible field names for the force password change flag
  const forcePasswordChange = 
    response.forcePasswordChange || 
    response.force_password_change || 
    response.requirePasswordChange || 
    response.require_password_change || 
    response.mustChangePassword || 
    false;
  
  console.log('[DEBUG] Force password change flag:', forcePasswordChange);
  
  return {
    id: String(response.userId || response.id || ''),
    name: response.userName || response.name || '', // Backend sends 'userName'
    email: response.email || response.userName || '', // Use userName as fallback
    userName: response.userName,
    role: response.role, // Single role from backend
    roles: Array.isArray(response.roles) ? response.roles : [response.role].filter(Boolean), // Convert to array
    societyId: String(response.societyId || response.society_id || ''),
    societyName: response.societyName || response.society_name, // Society name from backend
    forcePasswordChange: Boolean(forcePasswordChange), // Explicitly convert to boolean
    createdAt: response.createdAt || new Date().toISOString(),
    updatedAt: response.updatedAt || new Date().toISOString(),
  };
};

export const authApi = {
  async login(credentials: LoginCredentials): Promise<{ auth: AuthResponse; user: User }> {
    const response = await apiClient.post<ApiResponse<AuthResponse>>('/auth/login', credentials);
    const authResponse = response.data.data;
    return {
      auth: authResponse,
      user: authResponseToUser(authResponse),
    };
  },

  async register(credentials: RegisterCredentials): Promise<{ auth: AuthResponse; user: User }> {
    const response = await apiClient.post<ApiResponse<AuthResponse>>('/auth/register', credentials);
    const authResponse = response.data.data;
    const user = authResponseToUser(authResponse);
    // Enrich user with data from credentials since they were just created
    user.name = credentials.name;
    user.email = credentials.email;
    return {
      auth: authResponse,
      user,
    };
  },

  async logout(): Promise<void> {
    try {
      await apiClient.post('/auth/logout');
      console.log('[AUTH] Logout successful');
    } catch (error) {
      // Even if logout endpoint fails, we still want to clear local state
      // So we don't throw here - let the caller handle the cleanup
      console.warn('[AUTH] Logout endpoint error (but clearing local state):', error);
    }
  },

  async refresh(): Promise<AuthResponse> {
    const response = await apiClient.post<ApiResponse<AuthResponse>>('/auth/refresh', {}, {
      withCredentials: true
    });
    return response.data.data;
  },

  /**
   * Fetch current user from /auth/me endpoint
   * This ensures we always have the latest user data including forcePasswordChange flag
   */
  async getMe(): Promise<User> {
    try {
      const response = await apiClient.get<ApiResponse<User>>('/auth/me');
      console.log('[AUTH] User data from /auth/me:', response.data.data);
      return response.data.data;
    } catch (error: any) {
      // Log the error for debugging
      console.error('[AUTH] Failed to fetch user from /auth/me:', error.response?.status, error.message);
      
      // If the request failed, re-throw to let caller handle it
      throw error;
    }
  }
};
