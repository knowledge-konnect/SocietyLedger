import { PaginatedResponse, PaginationParams, ApiResponse } from '../types/api';
import { User } from '../types/auth';
import apiClient from './client';

export interface HealthStatus {
  status: 'healthy' | 'unhealthy';
  version: string;
  timestamp: string;
  services: {
    database: 'up' | 'down';
    cache: 'up' | 'down';
    storage: 'up' | 'down';
  };
}

export const adminApi = {
  async getAllUsers(params?: PaginationParams): Promise<PaginatedResponse<User>> {
    const response = await apiClient.get<ApiResponse<PaginatedResponse<User>>>('/admin/users', { params });
    return response.data.data;
  },

  async impersonateUser(societyId: string): Promise<{ accessToken: string }> {
    const response = await apiClient.post<ApiResponse<{ accessToken: string }>>(
      `/admin/societies/${societyId}/impersonate`
    );
    return response.data.data;
  },

  async getHealthStatus(): Promise<HealthStatus> {
    const response = await apiClient.get<ApiResponse<HealthStatus>>('/admin/health');
    return response.data.data;
  }
};
