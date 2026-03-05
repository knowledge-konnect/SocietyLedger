import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { ApiError } from '../types/api';

// Centralize API base URL: prefer Vite env, then React-style env, then fallback to localhost
const BASE_API_URL: string =
  (import.meta as any).env?.VITE_APP_API_URL ||
  (import.meta as any).env?.REACT_APP_API_URL ||
  'https://localhost:7110';

class Mutex {
  private locked = false;
  private queue: Array<() => void> = [];

  async acquire(): Promise<void> {
    if (!this.locked) {
      this.locked = true;
      return;
    }
    return new Promise(resolve => this.queue.push(resolve));
  }

  release(): void {
    const next = this.queue.shift();
    if (next) {
      next();
    } else {
      this.locked = false;
    }
  }
}

const mutex = new Mutex();
let isRefreshing = false;
let refreshSubscribers: Array<(token: string) => void> = [];

function onRefreshed(token: string): void {
  refreshSubscribers.forEach(callback => callback(token));
  refreshSubscribers = [];
}

function addRefreshSubscriber(callback: (token: string) => void): void {
  refreshSubscribers.push(callback);
}

export const apiClient: AxiosInstance = axios.create({
  baseURL: BASE_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    await mutex.acquire();

    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }

    mutex.release();
    return config;
  },
  error => {
    mutex.release();
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  response => response,
  async (error: AxiosError<ApiError>) => {
    const originalRequest = error.config;
    
    if (!originalRequest) {
      return Promise.reject(error);
    }

    // If the error is not 401 or it's already a retry, reject
    if (
      error.response?.status !== 401 ||
      (originalRequest as any)._retry
    ) {
      return Promise.reject(error);
    }

    // If there's no token, this is a login attempt with wrong credentials
    // Don't try to refresh - just reject the error
    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
      return Promise.reject(error);
    }

    if (!isRefreshing) {
      isRefreshing = true;

      try {
        const response = await axios.post(
          `${BASE_API_URL}/auth/refresh`,
          {},
          { withCredentials: true }
        );

        const { accessToken } = response.data;
        localStorage.setItem('accessToken', accessToken);
        
        onRefreshed(accessToken);
        isRefreshing = false;

        return apiClient(originalRequest);
      } catch (refreshError) {
        isRefreshing = false;
        // Clear token and refresh state
        localStorage.removeItem('accessToken');
        localStorage.removeItem('user');
        localStorage.removeItem('societyId');
        // Only redirect to login if this is a token refresh failure
        // (meaning user had a valid session that expired)
        window.location.href = '/login?reason=session_expired';
        return Promise.reject(refreshError);
      }
    }

    // If a refresh is already in progress, wait for it
    return new Promise(resolve => {
      addRefreshSubscriber(token => {
        originalRequest.headers.Authorization = `Bearer ${token}`;
        resolve(apiClient(originalRequest));
      });
    });
  }
);

export const handleApiError = (error: unknown): ApiError => {
  if (axios.isAxiosError(error) && error.response?.data) {
    return error.response.data as ApiError;
  }
  return {
    code: 'UNKNOWN_ERROR',
    message: 'An unexpected error occurred',
  };
};

export default apiClient;
