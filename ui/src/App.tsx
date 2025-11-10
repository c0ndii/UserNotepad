import "./App.css";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { LoginPage } from "./pages/Login";
import { RegisterPage } from "./pages/Register";
import { NotepadPage } from "./pages/Notepad";
import { AuthProvider } from "./providers/AuthProvider";
import { Navbar } from "./components/Navbar";
import { Box, CircularProgress } from "@mui/material";
import { useMe } from "./hooks/useMe";

function Content() {
  const { isLoading } = useMe();

  if (isLoading) return <CircularProgress size={50} color="primary" />;

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/" element={<NotepadPage />} />
    </Routes>
  );
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Box
          sx={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}
        >
          <Navbar />
          <Box
            sx={{
              flexGrow: 1,
              minHeight: "calc(100vh - 64px)",
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
            }}
          >
            <Content />
          </Box>
        </Box>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
