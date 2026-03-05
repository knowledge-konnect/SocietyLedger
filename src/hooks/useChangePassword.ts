import { useMutation } from '@tanstack/react-query';
import { usersApi, ChangePasswordDto } from '../api/usersApi';
import { handleApiError } from '../api/client';

export const useChangePassword = () => {
  return useMutation<void, Error, ChangePasswordDto>({
    mutationFn: async (payload) => {
      try {
        await usersApi.changePassword(payload);
      } catch (error) {
        throw handleApiError(error);
      }
    },
  });
};
