import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { usersApi, CreateUserDto, UpdateUserDto, User, CreateUserResponse } from '../api/usersApi';
import { PaginationParams } from '../types/api';

export const useUsers = (params?: PaginationParams) => {
  return useQuery<User[]>({
    queryKey: ['users', params],
    queryFn: () => usersApi.getUsers(params)
  });
};

export const useUser = (id: string) => {
  return useQuery<User>({
    queryKey: ['users', id],
    queryFn: () => usersApi.getUserById(id),
    enabled: !!id
  });
};

export const useCreateUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation<CreateUserResponse, Error, CreateUserDto>({
    mutationFn: (payload) => usersApi.createUser(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    }
  });
};

export const useUpdateUser = (id: string) => {
  const queryClient = useQueryClient();
  
  return useMutation<User, Error, UpdateUserDto>({
    mutationFn: (payload) => usersApi.updateUser(id, payload),
    onSuccess: (updatedUser) => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.setQueryData(['users', id], updatedUser);
    }
  });
};

export const useDeleteUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation<void, Error, string>({
    mutationFn: (userId) => usersApi.deleteUser(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    }
  });
};
