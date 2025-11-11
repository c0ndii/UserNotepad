import { useQuery } from "@tanstack/react-query";
import { api } from "../api/api";

export interface MeResponse {
  nickname: string;
}

export const useMe = () => {
  const tokenExpires = localStorage.getItem("tokenExpires");
  let refetchInterval = 0;
  if (tokenExpires) {
    refetchInterval = new Date(tokenExpires).getTime() - Date.now() + 60000;
  }

  return useQuery<MeResponse>({
    queryKey: ["me"],
    queryFn: async () => {
      const res = await api.get<MeResponse>("/me");
      return res.data;
    },
    retry: false,
    staleTime: 5 * 60 * 1000,
    refetchInterval: refetchInterval > 0 ? refetchInterval : false,
    refetchOnWindowFocus: false,
    refetchOnMount: false,
  });
};
