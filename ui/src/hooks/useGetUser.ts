import { useQuery } from "@tanstack/react-query";
import { api } from "../api/api";
import type { UserDto } from "../types/user";

export const useGetUser = (id: string) => {
  const { data, isLoading, isError, error, refetch } = useQuery<UserDto>({
    queryKey: ["user", id],
    queryFn: async () => {
      const res = await api.get<UserDto>(`/users/${id}`);
      return res.data;
    },
    enabled: !!id,
    retry: false,
  });

  return {
    user: data,
    isLoading,
    isError,
    error,
    refetch,
  };
};
