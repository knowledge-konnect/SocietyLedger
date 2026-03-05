import { useMutation } from '@tanstack/react-query';
import apiClient from '../../lib/api';

interface LoginData {
  email: string;
  password: string;
}

interface SignupData {
  name: string;
  email: string;
  password: string;
  societyName: string;
  societyAddress: string;
}

interface AuthResponse {
  token: string;
  user: {
    id: string;
    email: string;
    name: string;
    role: string;
    societyId: string;
  };
}

export function useLogin() {
  return useMutation({
    mutationFn: async (data: LoginData): Promise<AuthResponse> => {
      const response = await apiClient.post('/auth/login', data);
      return response.data;
    },
    onSuccess: (data) => {
      localStorage.setItem('auth_token', data.token);
      localStorage.setItem('user', JSON.stringify(data.user));
    },
  });
}

export function useSignup() {
  return useMutation({
    mutationFn: async (data: SignupData): Promise<AuthResponse> => {
      const response = await apiClient.post('/auth/signup', data);
      return response.data;
    },
    onSuccess: (data) => {
      localStorage.setItem('auth_token', data.token);
      localStorage.setItem('user', JSON.stringify(data.user));
    },
  });
}

export function useForgotPassword() {
  return useMutation({
    mutationFn: async (email: string) => {
      const response = await apiClient.post('/auth/forgot-password', { email });
      return response.data;
    },
  });
}
