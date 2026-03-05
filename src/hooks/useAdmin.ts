import { useMutation, useQuery } from '@tanstack/react-query';
import { adminApi } from '../api/adminApi';
import { PaginationParams } from '../types/api';

export const useAdminUsers = (params?: PaginationParams) => {
  return useQuery({
    queryKey: ['admin', 'users', params],
    queryFn: () => adminApi.getAllUsers(params)
  });
};

export const useImpersonateUser = () => {
  return useMutation({
    mutationFn: (societyId: string) => adminApi.impersonateUser(societyId)
  });
};

export const useHealthStatus = () => {
  return useQuery({
    queryKey: ['admin', 'health'],
    queryFn: () => adminApi.getHealthStatus(),
    refetchInterval: 30000 // Refetch every 30 seconds
  });
};
