import { useQuery } from '@tanstack/react-query';
import apiClient from '../../lib/api';
import { DashboardStats, ChartData, Activity } from '../../types';

interface DashboardData {
  stats: DashboardStats;
  chartData: ChartData[];
  recentActivity: Activity[];
}

export function useDashboard(societyId: string, month: string) {
  return useQuery({
    queryKey: ['dashboard', societyId, month],
    queryFn: async (): Promise<DashboardData> => {
      const response = await apiClient.get(`/dashboard`, {
        params: { societyId, month },
      });
      return response.data;
    },
    enabled: !!societyId,
  });
}
