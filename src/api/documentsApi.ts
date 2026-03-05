import { PaginatedResponse, PaginationParams, ApiResponse } from '../types/api';
import apiClient from './client';

export interface Document {
  id: string;
  name: string;
  type: string;
  size: number;
  url: string;
  createdAt: string;
}

export const documentsApi = {
  async uploadDocument(file: File): Promise<Document> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.post<ApiResponse<Document>>('/documents', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data.data;
  },

  async getDocuments(params?: PaginationParams): Promise<PaginatedResponse<Document>> {
    const response = await apiClient.get<ApiResponse<PaginatedResponse<Document>>>('/documents', { params });
    return response.data.data;
  },

  async downloadDocument(id: string): Promise<Blob> {
    const response = await apiClient.get(`/documents/${id}`, {
      responseType: 'blob'
    });
    return response.data;
  },

  async deleteDocument(id: string): Promise<void> {
    await apiClient.delete(`/documents/${id}`);
  }
};
