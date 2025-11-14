import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  TablePagination,
  CircularProgress,
  Box,
  IconButton,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import { useGetUsers } from "../hooks/useGetUsers";
import { theme } from "../main";
import { formatSex } from "../helpers/sexHelper";
import { useState } from "react";
import { UserModal } from "./UserModal";
import { AttributeTypeEnum } from "../types/user";
import { ConfirmDeleteModal } from "./ConfirmDeleteModal";

export const UsersTable = () => {
  const { users, totalCount, page, pageSize, setPage, isLoading } =
    useGetUsers();

  const [selectedUserId, setSelectedUserId] = useState<string | undefined>(
    undefined
  );
  const [userModalOpen, setUserModalOpen] = useState(false);

  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [deleteUserId, setDeleteUserId] = useState<string | null>(null);
  const [deleteUserFullName, setDeleteUserFullName] = useState<string>("");

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage + 1);
  };

  const handleRowClick = (id: string) => {
    setSelectedUserId(id);
    setUserModalOpen(true);
  };

  const handleCloseUserModal = () => {
    setUserModalOpen(false);
    setSelectedUserId(undefined);
  };

  const handleOpenDeleteModal = (
    id: string,
    name: string,
    surname: string,
    e: React.MouseEvent
  ) => {
    e.stopPropagation();
    setDeleteUserId(id);
    setDeleteUserFullName(`${name} ${surname}`);
    setDeleteModalOpen(true);
  };

  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <TableContainer
        component={Paper}
        sx={{
          maxHeight: 520,
          overflowY: "auto",
          boxShadow: "none",
        }}
      >
        <Table stickyHeader>
          <TableHead sx={{ backgroundColor: theme.palette.primary.main }}>
            <TableRow>
              <TableCell
                sx={{
                  backgroundColor: theme.palette.primary.main,
                  color: "#fff",
                  fontWeight: "bold",
                  fontSize: "1rem",
                }}
              >
                Name
              </TableCell>
              <TableCell
                sx={{
                  backgroundColor: theme.palette.primary.main,
                  color: "#fff",
                  fontWeight: "bold",
                  fontSize: "1rem",
                }}
              >
                Surname
              </TableCell>
              <TableCell
                sx={{
                  backgroundColor: theme.palette.primary.main,
                  color: "#fff",
                  fontWeight: "bold",
                  fontSize: "1rem",
                }}
              >
                Birth date
              </TableCell>
              <TableCell
                sx={{
                  backgroundColor: theme.palette.primary.main,
                  color: "#fff",
                  fontWeight: "bold",
                  fontSize: "1rem",
                }}
              >
                Sex
              </TableCell>
              <TableCell
                sx={{
                  backgroundColor: theme.palette.primary.main,
                  color: "#fff",
                  fontWeight: "bold",
                  fontSize: "1rem",
                }}
              >
                Attributes
              </TableCell>
              <TableCell
                sx={{
                  backgroundColor: theme.palette.primary.main,
                  color: "#fff",
                  fontWeight: "bold",
                  fontSize: "1rem",
                }}
              >
                Actions
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((user) => (
              <TableRow
                key={user.id}
                sx={{
                  cursor: "pointer",
                  "&:hover": {
                    backgroundColor: theme.palette.secondary.main,
                    color: "#fff",
                  },
                }}
                onClick={() => handleRowClick(user.id)}
              >
                <TableCell>{user.name}</TableCell>
                <TableCell>{user.surname}</TableCell>
                <TableCell>
                  {new Date(user.birthDate).toLocaleDateString()}
                </TableCell>
                <TableCell>{formatSex(user.sex)}</TableCell>
                <TableCell>
                  {user.attributes?.map((a, index) => (
                    <span key={index}>
                      <strong>{a.key}</strong>:{" "}
                      {a.valueType === AttributeTypeEnum.Date
                        ? new Date(a.value).toLocaleDateString()
                        : a.value}
                      {index < user.attributes!.length - 1 ? ", " : ""}
                    </span>
                  ))}
                </TableCell>
                <TableCell>
                  <IconButton
                    onClick={(e) =>
                      handleOpenDeleteModal(user.id, user.name, user.surname, e)
                    }
                    sx={{
                      borderRadius: 2,
                      color: theme.palette.error.main,
                      transition: "0.2s",
                      "&:hover": {
                        backgroundColor: "rgba(211, 47, 47, 0.15)",
                        color: theme.palette.error.dark,
                      },
                    }}
                  >
                    <DeleteIcon color="error" />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        sx={{
          backgroundColor: theme.palette.primary.main,
          color: "#fff",
          fontSize: "1.1rem",
          fontWeight: 600,
          borderBottomLeftRadius: 4,
          borderBottomRightRadius: 4,
        }}
        component="div"
        count={totalCount}
        page={page - 1}
        onPageChange={handleChangePage}
        rowsPerPage={pageSize}
        rowsPerPageOptions={[pageSize]}
      />
      {userModalOpen && (
        <UserModal
          open={userModalOpen}
          onClose={handleCloseUserModal}
          userId={selectedUserId}
        />
      )}

      <ConfirmDeleteModal
        open={deleteModalOpen}
        onClose={() => setDeleteModalOpen(false)}
        id={deleteUserId!}
        fullName={deleteUserFullName}
      />
    </Box>
  );
};
