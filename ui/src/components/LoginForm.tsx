import {
  Box,
  TextField,
  Button,
  Typography,
  CircularProgress,
} from "@mui/material";
import { useSnackbar } from "../hooks/useSnackbar";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { theme } from "../main";
import { useLogin } from "../hooks/useLogin";

const loginSchema = z.object({
  username: z.string().nonempty("Username is required"),
  password: z.string().nonempty("Password is required"),
});

type LoginFormValues = z.infer<typeof loginSchema>;

interface LoginFormProps {
  onSuccess?: () => void;
}

export const LoginForm = ({ onSuccess }: LoginFormProps) => {
  const { mutateAsync, isPending } = useLogin();
  const { showMessage } = useSnackbar();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    mode: "all",
  });

  const onSubmit = async (data: LoginFormValues) => {
    try {
      await mutateAsync(data);
      showMessage("Login successful", "success");
      if (onSuccess) {
        onSuccess();
      }
    } catch (error: unknown) {
      showMessage("Error occured", "error");
      console.log(error);
    }
  };

  return (
    <Box
      component="form"
      onSubmit={handleSubmit(onSubmit)}
      sx={{
        width: "30vw",
        padding: 4,
        borderRadius: 4,
        border: `2px solid ${theme.palette.secondary.main}`,
        backgroundColor: theme.palette.background.paper,
        transition: "all 0.2s ease",
        "&:hover": {
          borderColor: `${theme.palette.primary.main}`,
          boxShadow: `0 0 10px ${theme.palette.primary.main}50`,
        },
      }}
      noValidate
    >
      <Typography variant="h5" className="pb-4 text-center font-semibold">
        Login
      </Typography>

      <Box className="flex flex-col gap-4">
        <TextField
          label="Username"
          {...register("username")}
          error={!!errors.username}
          helperText={errors.username?.message}
          fullWidth
        />
        <TextField
          label="Password"
          type="password"
          {...register("password")}
          error={!!errors.password}
          helperText={errors.password?.message}
          fullWidth
        />

        <Button
          type="submit"
          variant="contained"
          color="primary"
          disabled={isPending}
          fullWidth
        >
          {isPending ? (
            <CircularProgress size={24} color="inherit" />
          ) : (
            "Submit"
          )}
        </Button>
      </Box>
    </Box>
  );
};
