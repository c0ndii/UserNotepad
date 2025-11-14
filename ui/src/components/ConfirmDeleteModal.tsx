import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
  CircularProgress,
} from "@mui/material";
import { useDeleteUser } from "../hooks/useDeleteUser";
import { useSnackbar } from "../hooks/useSnackbar";

interface Props {
  open: boolean;
  onClose: () => void;
  id: string;
  fullName: string;
}

export const ConfirmDeleteModal = ({ open, onClose, id, fullName }: Props) => {
  const { mutateAsync, isPending } = useDeleteUser();
  const { showMessage } = useSnackbar();

  const handleDelete = async () => {
    await mutateAsync(id, {
      onSuccess: () => {
        showMessage("User deleted successfully", "success");
        onClose();
      },
      onError: (error: Error) => {
        showMessage(`Error occurred`, "error");
        console.log(error);
      },
    });
  };

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>Delete user</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to delete <strong>{fullName}</strong>?
        </DialogContentText>
      </DialogContent>

      <DialogActions>
        <Button onClick={onClose} disabled={isPending}>
          Cancel
        </Button>

        <Button
          onClick={handleDelete}
          color="error"
          variant="contained"
          disabled={isPending}
          startIcon={
            isPending ? <CircularProgress size={24} color="inherit" /> : null
          }
        >
          {isPending ? (
            <CircularProgress size={24} color="inherit" />
          ) : (
            "Confirm"
          )}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
