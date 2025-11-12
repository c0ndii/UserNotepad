import { useEffect, useState, type ReactNode } from "react";
import { useMe, type MeResponse } from "../hooks/useMe";
import { AuthContext } from "../contexts/AuthContext";

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const { data, isLoading, isError } = useMe();
  const [user, setUser] = useState<MeResponse | null>(null);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    if (isLoading) return;

    if (isError || !data) {
      localStorage.removeItem("token");
      setUser(null);
    } else {
      setUser(data);
    }

    setReady(true);
  }, [data, isLoading, isError]);

  return (
    <AuthContext.Provider value={{ user, setUser, isLoading: !ready }}>
      {children}
    </AuthContext.Provider>
  );
};
