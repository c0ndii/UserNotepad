import { useMutation } from "@tanstack/react-query";
import { api } from "../api/api";
import { useAuth } from "./useAuth";

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  userNickname: string;
  jwtToken: string;
  jwtExpiration: string;
}

export const useLogin = () => {
  const { setUser } = useAuth();

  return useMutation<LoginResponse, unknown, LoginRequest>({
    mutationFn: async (data) => {
      const res = await api.post<LoginResponse>("/login", data);
      return res.data;
    },
    onSuccess: (data) => {
      localStorage.setItem("token", data.jwtToken);
      localStorage.setItem("tokenExpires", data.jwtExpiration);
      setUser({ nickname: data.userNickname });
    },
  });
};
