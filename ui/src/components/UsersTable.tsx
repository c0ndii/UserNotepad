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
} from "@mui/material";
import { useGetUsers } from "../hooks/useGetUsers";
import { theme } from "../main";
import { formatSex } from "../helpers/sexHelper";
import { useState } from "react";
import { UserModal } from "./UserModal";

export const UsersTable = () => {
  const { users, totalCount, page, pageSize, setPage, isLoading } =
    useGetUsers();

  const [selectedUserId, setSelectedUserId] = useState<string | undefined>(
    undefined
  );
  const [modalOpen, setModalOpen] = useState(false);

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage + 1);
  };

  const handleRowClick = (id: string) => {
    setSelectedUserId(id);
    setModalOpen(true);
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setSelectedUserId(undefined);
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
                      <strong>{a.key}</strong>: {a.value}
                      {index < user.attributes!.length - 1 ? ", " : ""}
                    </span>
                  ))}
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
      {modalOpen && (
        <UserModal
          open={modalOpen}
          onClose={handleCloseModal}
          userId={selectedUserId}
        />
      )}
    </Box>
  );
};
