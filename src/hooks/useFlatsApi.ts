import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { flatsApi, FlatDto, CreateFlatDto, UpdateFlatDto, FlatStatusDto } from '../api/flatsApi';

export function useFlats(societyId?: number) {
  return useQuery({
    queryKey: ['flats', societyId],
    queryFn: async (): Promise<FlatDto[]> => {
      if (!societyId) return [];
      return flatsApi.listBySociety(societyId);
    },
    enabled: !!societyId,
  });
}

export function useCreateFlat() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: CreateFlatDto) => {
      return flatsApi.createFlat(payload);
    },
    onSuccess: (_, variables) => {
      qc.invalidateQueries({ queryKey: ['flats', variables.societyId] });
    }
  });
}

export function useUpdateFlat() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: UpdateFlatDto) => {
      return flatsApi.updateFlat(payload);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['flats'] });
    }
  });
}

export function useDeleteFlat() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (publicId: string) => {
      return flatsApi.deleteFlat(publicId);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['flats'] });
    }
  });
}

export function useFlatStatuses() {
  return useQuery({
    queryKey: ['flat-statuses'],
    queryFn: async (): Promise<FlatStatusDto[]> => {
      return flatsApi.getStatuses();
    }
  });
}
