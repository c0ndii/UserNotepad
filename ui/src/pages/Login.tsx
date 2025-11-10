import { Box } from "@mui/material";
import { LoginForm } from "../components/LoginForm";
import { useNavigate } from "react-router-dom";

export const LoginPage = () => {
  const navigate = useNavigate();

  return (
    <Box>
      <LoginForm
        onSuccess={() => {
          navigate("/");
        }}
      />
    </Box>
  );
};
