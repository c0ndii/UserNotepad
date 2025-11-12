import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { UserDto, UserInput } from "../types/user";
import { api } from "../api/api";

export const useUpdateUser = (id: string) => {
  const queryClient = useQueryClient();

  return useMutation<UserDto, Error, UserInput>({
    mutationFn: async (data: UserInput) => {
      const res = await api.put<UserDto>(`/users/${id}`, data);
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["users"] });
    },
  });
};
