import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  announcementsApi,
  CreateAnnouncementDto,
  UpdateAnnouncementDto
} from '../api/announcementsApi';
import { PaginationParams } from '../types/api';

export const useAnnouncements = (params?: PaginationParams) => {
  return useQuery({
    queryKey: ['announcements', params],
    queryFn: () => announcementsApi.getAnnouncements(params)
  });
};

export const useCreateAnnouncement = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateAnnouncementDto) => 
      announcementsApi.createAnnouncement(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['announcements'] });
    }
  });
};

export const useUpdateAnnouncement = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: UpdateAnnouncementDto) =>
      announcementsApi.updateAnnouncement(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['announcements'] });
    }
  });
};

export const useDeleteAnnouncement = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => announcementsApi.deleteAnnouncement(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['announcements'] });
    }
  });
};
