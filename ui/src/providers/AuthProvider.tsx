import { useEffect, useState, type ReactNode } from "react";
import type { MeResponse } from "../hooks/useMe";
import { AuthContext } from "../contexts/AuthContext";
import { api } from "../api/api";

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<MeResponse | null>(null);

  useEffect(() => {
    (async () => {
      try {
        const res = await api.get<MeResponse>("/me");
        setUser(res.data);
      } catch {
        localStorage.removeItem("token");
        setUser(null);
      }
    })();
  }, []);

  return (
    <AuthContext.Provider value={{ user, setUser }}>
      {children}
    </AuthContext.Provider>
  );
};
