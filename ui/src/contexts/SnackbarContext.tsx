import { createContext } from "react";

export type SnackbarType = "success" | "error" | "info" | "warning";

interface SnackbarContextType {
  showMessage: (message: string, type?: SnackbarType) => void;
}

export const SnackbarContext = createContext<SnackbarContextType | undefined>(
  undefined
);
