import { Box, Button, Typography } from "@mui/material";
import { UsersTable } from "../components/UsersTable";
import { useState } from "react";
import { UserModal } from "../components/UserModal";

export const NotepadPage = () => {
  const [openModal, setOpenModal] = useState(false);

  return (
    <Box sx={{ width: "80%" }}>
      <Box
        component="div"
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          width: "100%",
          marginBottom: 2,
        }}
      >
        <Typography variant="h5">Users</Typography>{" "}
        <Box component="div" sx={{ display: "flex", gap: 2 }}>
          <Button onClick={() => setOpenModal(true)} variant="contained">
            Add user
          </Button>
          <Button variant="contained">Generate report</Button>
        </Box>
      </Box>
      <UsersTable />
      <UserModal open={openModal} onClose={() => setOpenModal(false)} />
    </Box>
  );
};
