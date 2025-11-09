import { createContext } from "react";
import { type MeResponse } from "../hooks/useMe";

interface AuthContextType {
  user: MeResponse | null;
  setUser: (user: MeResponse | null) => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);
