import { useQuery } from "@tanstack/react-query";
import { api } from "../api/api";

export interface MeResponse {
  nickname: string;
}

export const useMe = () => {
  return useQuery<MeResponse>({
    queryKey: ["me"],
    queryFn: async () => {
      const res = await api.get<MeResponse>("/me");
      return res.data;
    },
    retry: false,
    staleTime: 5 * 60 * 1000,
    refetchOnWindowFocus: false,
    refetchOnMount: false,
  });
};
