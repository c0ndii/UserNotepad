import { useMutation } from "@tanstack/react-query";
import { api } from "../api/api";

interface RegisterRequest {
  username: string;
  nickname: string;
  password: string;
  repeatPassword: string;
}

export const useRegister = () => {
  return useMutation({
    mutationFn: async (data: RegisterRequest) => {
      const res = await api.post("/register", data);
      return res.data;
    },
  });
};
