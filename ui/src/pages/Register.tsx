import { RegisterForm } from "../components/RegisterForm";
import { Box } from "@mui/material";
import { useNavigate } from "react-router-dom";

export const RegisterPage = () => {
  const navigate = useNavigate();

  return (
    <Box>
      <RegisterForm
        onSuccess={() => {
          navigate("/login");
        }}
      />
    </Box>
  );
};
