import { useQuery } from "@tanstack/react-query";
import { api } from "../api/api";

export const useUserReport = () => {
  const query = useQuery({
    queryKey: ["report"],
    queryFn: async () => {
      const response = await api.get("/users/report", { responseType: "blob" });

      const blob = new Blob([response.data], { type: "application/pdf" });
      const url = URL.createObjectURL(blob);

      let fileName = "report.pdf";
      const contentDisposition = response.headers["content-disposition"];
      if (contentDisposition) {
        const match = contentDisposition.match(/filename="?(.+?)"?$/);
        if (match && match[1]) {
          fileName = match[1].replace(/"/g, "");
        }
      }

      const link = document.createElement("a");
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      URL.revokeObjectURL(url);

      return fileName;
    },
    enabled: false,
  });

  return query;
};
