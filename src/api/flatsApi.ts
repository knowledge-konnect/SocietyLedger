import { ApiResponse } from '../types/api';
import apiClient from './client';

export interface FlatDto {
  id: number;
  publicId: string;
  societyId: number;
  flatNo: string;
  ownerName: string;
  contactMobile: string;
  contactEmail: string;
  maintenanceAmount: number;
  status?: string;
  statusId?: number;
  statusName?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFlatDto {
  societyId: number;
  flatNo: string;
  ownerName: string;
  contactMobile?: string;
  contactEmail?: string;
  maintenanceAmount?: number;
  statusId?: number;
}

export interface UpdateFlatDto {
  publicId: string;
  flatNo?: string;
  ownerName?: string;
  contactMobile?: string;
  contactEmail?: string;
  maintenanceAmount?: number;
  statusId?: number;
}

export interface FlatStatusDto {
  id: number;
  code: string;
  displayName: string;
}

export const flatsApi = {
  async createFlat(payload: CreateFlatDto): Promise<FlatDto> {
    const response = await apiClient.post<ApiResponse<FlatDto>>('/flats', payload);
    return response.data.data;
  },

  async getByPublicId(publicId: string): Promise<FlatDto> {
    const response = await apiClient.get<ApiResponse<FlatDto>>(`/flats/${publicId}`);
    return response.data.data;
  },

  async updateFlat(payload: UpdateFlatDto): Promise<FlatDto> {
    const response = await apiClient.put<ApiResponse<FlatDto>>('/flats', payload);
    return response.data.data;
  },
  async deleteFlat(publicId: string): Promise<void> {
    const response = await apiClient.delete<ApiResponse<null>>(`/flats/${publicId}`);
    // some APIs may return no data; return void
    return response.data.data as unknown as void;
  },
  async listBySociety(societyId: number): Promise<FlatDto[]> {
    const response = await apiClient.get<ApiResponse<FlatDto[]>>(`/flats/society/${societyId}`);
    return response.data.data;
  },

  async getStatuses(): Promise<FlatStatusDto[]> {
    const response = await apiClient.get<ApiResponse<FlatStatusDto[]>>('/flats/statuses');
    return response.data.data;
  },
  

};
