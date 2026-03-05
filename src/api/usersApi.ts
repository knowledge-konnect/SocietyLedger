import { PaginatedResponse, PaginationParams, ApiResponse } from '../types/api';
import apiClient from './client';
import { RoleId } from '../types/roles';

export interface User {
  publicId: string;
  name: string;
  email: string;
  mobile: string;
  roleId: RoleId;
  roleDisplayName: string;
  isActive: boolean;
  forcePasswordChange: boolean;
  lastLogin: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserDto {
  email: string;
  name: string;
  password: string;
  roleId: RoleId;
}

export interface CreateUserResponse extends User {
  temporaryPassword?: string;
}

export interface UpdateUserDto {
  publicId: string;
  name?: string;
  email?: string;
  mobile?: string;
  roleId?: RoleId;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export const usersApi = {
  async getUsers(params?: PaginationParams): Promise<User[]> {
    const response = await apiClient.get<ApiResponse<User[]>>('/users', { params });
    return response.data.data;
  },

  async getUserById(id: string): Promise<User> {
    const response = await apiClient.get<ApiResponse<User>>(`/users/${id}`);
    return response.data.data;
  },

  async createUser(payload: CreateUserDto): Promise<CreateUserResponse> {
    const response = await apiClient.post<ApiResponse<CreateUserResponse>>('/users', payload);
    return response.data.data;
  },

  async updateUser(id: string, payload: UpdateUserDto): Promise<User> {
    const response = await apiClient.put<ApiResponse<User>>(`/users/${id}`, payload);
    return response.data.data;
  },

  async changePassword(payload: ChangePasswordDto): Promise<void> {
    await apiClient.post('/auth/change-password', payload);
  },

  async deleteUser(id: string): Promise<void> {
    await apiClient.delete(`/users/${id}`);
  }
};
