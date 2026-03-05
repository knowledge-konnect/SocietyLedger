import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient from '../../lib/api';
import { Flat } from '../../types';

export function useFlats(societyId: string) {
  return useQuery({
    queryKey: ['flats', societyId],
    queryFn: async (): Promise<Flat[]> => {
      const response = await apiClient.get(`/societies/${societyId}/flats`);
      return response.data;
    },
    enabled: !!societyId,
  });
}

export function useImportFlats() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ societyId, data }: { societyId: string; data: any[] }) => {
      const response = await apiClient.post(`/societies/${societyId}/flats/import`, { data });
      return response.data;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['flats', variables.societyId] });
    },
  });
}

export function useFlatLedger(flatId: string) {
  return useQuery({
    queryKey: ['flat-ledger', flatId],
    queryFn: async () => {
      const response = await apiClient.get(`/flats/${flatId}/ledger`);
      return response.data;
    },
    enabled: !!flatId,
  });
}
