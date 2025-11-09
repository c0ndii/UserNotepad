// src/components/Navbar.tsx
import { Link } from "react-router-dom";
import { useMe } from "../hooks/useMe";

export const Navbar = () => {
  const { data: user } = useMe();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("nickname");
    window.location.href = "/login";
  };

  return (
    <nav className="bg-blue-500 text-white p-4 flex justify-between">
      <div className="font-bold">UserNotepad</div>
      <div>
        {user ? (
          <>
            <span className="mr-4">Witaj, {user.nickname}</span>
            <button
              onClick={handleLogout}
              className="bg-white text-blue-500 px-3 py-1 rounded"
            >
              Wyloguj
            </button>
          </>
        ) : (
          <>
            <Link to="/login" className="mr-4 hover:underline">
              Login
            </Link>
            <Link to="/register" className="hover:underline">
              Register
            </Link>
          </>
        )}
      </div>
    </nav>
  );
};
