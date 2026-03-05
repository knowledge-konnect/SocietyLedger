import { useMutation, useQueryClient, useQuery } from '@tanstack/react-query';
import apiClient from '../../lib/api';
import { Bill, Payment } from '../../types';

export function useGenerateBills() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: { societyId: string; period: string }) => {
      const response = await apiClient.post('/bills/generate', data);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['bills'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}

export function useBills(societyId: string) {
  return useQuery({
    queryKey: ['bills', societyId],
    queryFn: async (): Promise<Bill[]> => {
      const response = await apiClient.get(`/societies/${societyId}/bills`);
      return response.data;
    },
    enabled: !!societyId,
  });
}

export function useCreatePayment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: Omit<Payment, 'id'>) => {
      const response = await apiClient.post('/payments', data);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['bills'] });
      queryClient.invalidateQueries({ queryKey: ['payments'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}

export function usePayments(societyId: string) {
  return useQuery({
    queryKey: ['payments', societyId],
    queryFn: async (): Promise<Payment[]> => {
      const response = await apiClient.get(`/societies/${societyId}/payments`);
      return response.data;
    },
    enabled: !!societyId,
  });
}
