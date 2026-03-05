import { PaginatedResponse, PaginationParams, ApiResponse } from '../types/api';
import apiClient from './client';

export interface Announcement {
  id: string;
  title: string;
  content: string;
  type: 'NEWS' | 'EVENT' | 'NOTICE';
  date: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAnnouncementDto {
  title: string;
  content: string;
  type: Announcement['type'];
  date: string;
}

export interface UpdateAnnouncementDto {
  title?: string;
  content?: string;
  type?: Announcement['type'];
  date?: string;
}

export const announcementsApi = {
  async getAnnouncements(params?: PaginationParams): Promise<PaginatedResponse<Announcement>> {
    const response = await apiClient.get<ApiResponse<PaginatedResponse<Announcement>>>('/announcements', { params });
    return response.data.data;
  },

  async createAnnouncement(payload: CreateAnnouncementDto): Promise<Announcement> {
    const response = await apiClient.post<ApiResponse<Announcement>>('/announcements', payload);
    return response.data.data;
  },

  async updateAnnouncement(id: string, payload: UpdateAnnouncementDto): Promise<Announcement> {
    const response = await apiClient.put<ApiResponse<Announcement>>(`/announcements/${id}`, payload);
    return response.data.data;
  },

  async deleteAnnouncement(id: string): Promise<void> {
    await apiClient.delete(`/announcements/${id}`);
  }
};
