import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient from '../../lib/api';
import { Expense } from '../../types';

export function useExpenses(societyId: string, month?: string) {
  return useQuery({
    queryKey: ['expenses', societyId, month],
    queryFn: async (): Promise<Expense[]> => {
      const response = await apiClient.get(`/societies/${societyId}/expenses`, {
        params: month ? { month } : {},
      });
      return response.data;
    },
    enabled: !!societyId,
  });
}

export function useCreateExpense() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: Omit<Expense, 'id'>) => {
      const response = await apiClient.post(`/societies/${data.societyId}/expenses`, data);
      return response.data;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['expenses', variables.societyId] });
      queryClient.invalidateQueries({ queryKey: ['dashboard', variables.societyId] });
    },
  });
}
