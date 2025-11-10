import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { AppBar, Box, Button, Toolbar, Typography } from "@mui/material";
import { useSnackbar } from "../hooks/useSnackbar";

export const Navbar = () => {
  const { user, setUser } = useAuth();
  const { showMessage } = useSnackbar();
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("tokenExpires");
    setUser(null);
    showMessage("Logged out", "success");
    navigate("/login");
  };

  return (
    <Box sx={{ flexGrow: 0 }}>
      <AppBar position="static" color="primary">
        <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
          <Typography
            variant="h6"
            component={Link}
            to="/"
            sx={{ color: "inherit", textDecoration: "none" }}
          >
            UserNotepad
          </Typography>

          <Box>
            {!user ? (
              <>
                <Button color="inherit" component={Link} to="/login">
                  Login
                </Button>
                <Button color="inherit" component={Link} to="/register">
                  Register
                </Button>
              </>
            ) : (
              <>
                <Typography
                  variant="body1"
                  sx={{ mr: 2, display: "inline-block" }}
                >
                  Welcome, <strong>{user.nickname}</strong>
                </Typography>
                <Button color="inherit" onClick={handleLogout}>
                  Logout
                </Button>
              </>
            )}
          </Box>
        </Toolbar>
      </AppBar>
    </Box>
  );
};
