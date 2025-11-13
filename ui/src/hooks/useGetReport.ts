import { useQuery } from "@tanstack/react-query";
import { api } from "../api/api";
import { useState, useEffect } from "react";

export const useUserReport = () => {
  const [currentUrl, setCurrentUrl] = useState<string | null>(null);

  const query = useQuery({
    queryKey: ["report"],
    queryFn: async () => {
      const response = await api.get("/users/report", {
        responseType: "blob",
      });

      if (currentUrl) {
        URL.revokeObjectURL(currentUrl);
      }

      const blob = new Blob([response.data], { type: "application/pdf" });
      const url = URL.createObjectURL(blob);
      setCurrentUrl(url);

      return { blob, url };
    },
    enabled: false,
  });

  useEffect(() => {
    return () => {
      if (currentUrl) {
        URL.revokeObjectURL(currentUrl);
      }
    };
  }, [currentUrl]);

  return query;
};
