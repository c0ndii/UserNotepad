import {
  Box,
  TextField,
  Button,
  Typography,
  CircularProgress,
} from "@mui/material";
import { useRegister } from "../hooks/useRegister";
import { useSnackbar } from "../hooks/useSnackbar";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { theme } from "../main";
import type { AxiosError } from "axios";

const registerSchema = z
  .object({
    username: z.string().nonempty("Username is required"),
    nickname: z.string().nonempty("Nickname is required"),
    password: z.string().min(8, "Password must be at least 8 characters"),
    repeatPassword: z.string().min(8, "Password must be equal"),
  })
  .refine((data) => data.password === data.repeatPassword, {
    message: "Password must be equal",
    path: ["repeatPassword"],
  });

type RegisterFormValues = z.infer<typeof registerSchema>;

interface RegisterFormProps {
  onSuccess?: () => void;
}

export const RegisterForm = ({ onSuccess }: RegisterFormProps) => {
  const { mutateAsync, isPending } = useRegister();
  const { showMessage } = useSnackbar();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    mode: "all",
  });

  const onSubmit = async (data: RegisterFormValues) => {
    try {
      await mutateAsync(data);
      showMessage("Operator registered", "success");
      if (onSuccess) {
        onSuccess();
      }
    } catch (error) {
      const axiosError = error as AxiosError;
      if (axiosError.response?.status === 409) {
        showMessage("Username is already taken", "warning");
        return;
      }

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
        Register
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
          label="Nickname"
          {...register("nickname")}
          error={!!errors.nickname}
          helperText={errors.nickname?.message}
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
        <TextField
          label="Repeat password"
          type="password"
          {...register("repeatPassword")}
          error={!!errors.repeatPassword}
          helperText={errors.repeatPassword?.message}
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
