import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../api/api";
import type { UserDto, UserInput } from "../types/user";

export const useAddUser = () => {
  const queryClient = useQueryClient();

  return useMutation<UserDto, Error, UserInput>({
    mutationFn: async (data: UserInput) => {
      const res = await api.post<UserDto>("/users", data);
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["users"] });
    },
  });
};
