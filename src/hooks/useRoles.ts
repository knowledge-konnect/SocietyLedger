import { useQuery } from '@tanstack/react-query';
import { rolesApi, Role } from '../api/rolesApi';

export const useRoles = () => {
  return useQuery<Role[]>({
    queryKey: ['roles'],
    queryFn: () => rolesApi.getRoles()
  });
};

export const useRole = (code: string) => {
  return useQuery<Role>({
    queryKey: ['roles', code],
    queryFn: () => rolesApi.getRoleByCode(code),
    enabled: !!code
  });
};
