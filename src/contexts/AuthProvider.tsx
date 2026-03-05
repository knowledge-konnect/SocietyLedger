import React, { createContext, useContext, useEffect, useState } from 'react';
import { authApi } from '../api/authApi';
import { subscriptionApi } from '../api/subscriptionApi';
import { AuthState, LoginCredentials, RegisterCredentials, User } from '../types/auth';
import { handleApiError } from '../api/client';

interface AuthContextType extends AuthState {
  login: (credentials: LoginCredentials) => Promise<void>;
  register: (credentials: RegisterCredentials) => Promise<void>;
  logout: () => Promise<void>;
  updateUser: (user: Partial<User>) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [state, setState] = useState<AuthState>({
    user: null,
    accessToken: localStorage.getItem('accessToken'),
    isAuthenticated: false,
    isLoading: true,
  });

  const setAuthState = (user: User | null, token: string | null) => {
    setState({
      user,
      accessToken: token,
      isAuthenticated: !!user && !!token,
      isLoading: false,
    });

    if (token) {
      localStorage.setItem('accessToken', token);
      if (user) {
        localStorage.setItem('user', JSON.stringify(user));
        // Also persist societyId separately for legacy code that expects it
        if (user.societyId) {
          try {
            localStorage.setItem('societyId', String(user.societyId));
          } catch {}
        }
      }
    } else {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('user');
      localStorage.removeItem('societyId');
    }
  };

  const login = async (credentials: LoginCredentials) => {
    try {
      const { auth, user } = await authApi.login(credentials);
      console.log('Login successful, user object:', user); // Debug log
      console.log('[DEBUG] User forcePasswordChange flag:', user.forcePasswordChange);
      setAuthState(user, auth.accessToken);
    } catch (error) {
      setAuthState(null, null);
      throw handleApiError(error);
    }
  };

  const register = async (credentials: RegisterCredentials) => {
    try {
      const { auth, user } = await authApi.register(credentials);
      setAuthState(user, auth.accessToken);

      // Automatically create trial subscription for new users
      try {
        const trialResult = await subscriptionApi.createTrial();
        if (trialResult.succeeded) {
          console.log('Trial subscription created successfully:', trialResult.trialEnd);
        } else {
          console.warn('Failed to create trial subscription:', trialResult.error);
          // Don't fail registration if trial creation fails
        }
      } catch (trialError) {
        console.warn('Error creating trial subscription:', trialError);
        // Don't fail registration if trial creation fails
      }
    } catch (error) {
      setAuthState(null, null);
      throw handleApiError(error);
    }
  };

  const logout = async () => {
    // Clear auth state immediately to prevent any API calls with old token
    setAuthState(null, null);
    try {
      // Attempt to notify server of logout (this may fail if token is invalid)
      await authApi.logout();
    } catch (error) {
      // Logout failed on server, but local state is already cleared
      console.error('Server logout failed:', error);
    }
  };

  const updateUser = (updates: Partial<User>) => {
    if (state.user) {
      const updatedUser = { ...state.user, ...updates };
      setAuthState(updatedUser, state.accessToken);
    }
  };

  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('accessToken');
      
      if (!token) {
        setState(prev => ({ ...prev, isLoading: false }));
        return;
      }

      try {
        // Always fetch fresh user data from the server to ensure flags like forcePasswordChange are current
        const user = await authApi.getMe();
        setAuthState(user, token);
      } catch (error) {
        // If /auth/me fails but token exists, still try to use stored user data
        const storedUser = localStorage.getItem('user');
        if (storedUser) {
          try {
            const user = JSON.parse(storedUser);
            // Use stored user but mark that refresh failed
            setAuthState(user, token);
            console.warn('Using cached user data after server check failed:', error);
            return;
          } catch (parseError) {
            // Invalid stored user data
          }
        }
        // If no valid user data available, clear auth state
        setAuthState(null, null);
        console.error('Auth initialization failed:', error);
      }
    };

    initAuth();
  }, []);

  const value = {
    ...state,
    login,
    register,
    logout,
    updateUser,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
