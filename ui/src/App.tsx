import { useEffect } from "react";
import "./App.css";
import { useAuth } from "./hooks/useAuth";
import { useMe } from "./hooks/useMe";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { LoginPage } from "./pages/Login";
import { RegisterPage } from "./pages/Register";
import { NotepadPage } from "./pages/Notepad";
import { AuthProvider } from "./providers/AuthProvider";
import { Navbar } from "./components/Navbar";
import { Box } from "@mui/material";

function Content() {
  const { setUser } = useAuth();
  const { data, isLoading } = useMe();

  useEffect(() => {
    if (data) {
      setUser(data);
    }
  }, [data, setUser]);

  if (isLoading) return <div>Loading...</div>;

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
          <Box sx={{ flexGrow: 1 }}>
            <Content />
          </Box>
        </Box>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
