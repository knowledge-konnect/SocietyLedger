import { PaginatedResponse, PaginationParams, ApiResponse } from '../types/api';
import apiClient from './client';

export interface Society {
  id: string;
  name: string;
  address: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateSocietyDto {
  name: string;
  address: string;
}

export interface UpdateSocietyDto {
  name?: string;
  address?: string;
}

export const societiesApi = {
  async getSocieties(params?: PaginationParams): Promise<PaginatedResponse<Society>> {
    const response = await apiClient.get<ApiResponse<PaginatedResponse<Society>>>('/societies', { params });
    return response.data.data;
  },

  async getSocietyById(id: string): Promise<Society> {
    const response = await apiClient.get<ApiResponse<Society>>(`/societies/${id}`);
    return response.data.data;
  },

  async createSociety(payload: CreateSocietyDto): Promise<Society> {
    const response = await apiClient.post<ApiResponse<Society>>('/societies', payload);
    return response.data.data;
  },

  async updateSociety(id: string, payload: UpdateSocietyDto): Promise<Society> {
    const response = await apiClient.put<ApiResponse<Society>>(`/societies/${id}`, payload);
    return response.data.data;
  }
};
