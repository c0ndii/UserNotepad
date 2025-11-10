import { useState } from "react";
import { Box, TextField, Button, Typography } from "@mui/material";
import { useRegister } from "../hooks/useRegister";
import { useSnackbar } from "../hooks/useSnackbar";

export const RegisterForm = ({ onSuccess }: { onSuccess?: () => void }) => {
  const [username, setUsername] = useState("");
  const [nickname, setNickname] = useState("");
  const [password, setPassword] = useState("");
  const [repeatPassword, setRepeatPassword] = useState("");
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  const { showMessage } = useSnackbar();
  const mutation = useRegister();

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!username.trim()) newErrors.username = "Username is required";
    if (!nickname.trim()) newErrors.nickname = "Nickname is required";
    if (!password) newErrors.password = "Password is required";
    else if (password.length < 8)
      newErrors.password = "Password must be at least 8 characters long";
    if (password !== repeatPassword)
      newErrors.repeatPassword = "Passwords do not match";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    mutation.mutate(
      { username, nickname, password, repeatPassword },
      {
        onSuccess: () => {
          showMessage("Operator registered", "success");
          if (onSuccess) onSuccess();
        },
        onError: (err: unknown) => {
          console.error(err);
          showMessage("Registration error", "error");
        },
      }
    );
  };

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      className="max-w-md w-full mx-auto gap-4 mt-10 p-8 rounded-lg shadow-lg"
    >
      <Typography variant="h5" className="mb-6 text-center font-bold">
        Registration
      </Typography>

      <TextField
        label="Username"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        fullWidth
        required
        error={!!errors.username}
        helperText={errors.username}
        className="mb-4"
      />

      <TextField
        label="Nickname"
        value={nickname}
        onChange={(e) => setNickname(e.target.value)}
        fullWidth
        required
        error={!!errors.nickname}
        helperText={errors.nickname}
        className="mb-4"
      />

      <TextField
        label="Password"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        fullWidth
        required
        error={!!errors.password}
        helperText={errors.password}
        className="mb-4"
      />

      <TextField
        label="Repeat password"
        type="password"
        value={repeatPassword}
        onChange={(e) => setRepeatPassword(e.target.value)}
        fullWidth
        required
        error={!!errors.repeatPassword}
        helperText={errors.repeatPassword}
        className="mb-6"
      />

      <Button type="submit" variant="contained" color="primary" fullWidth>
        "Register"
      </Button>
    </Box>
  );
};
