import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';

import { BrowserRouter, Route, Routes } from "react-router";
import NavBar from "./Components/UtilityComponents/NavBar";
import UsersPage from './Components/UsersPage';
import ItemsPage from './Components/ItemsPage';
import Recover from './Components/Recover';
import Home from './Components/Home';
import LoginPage from './Components/LoginPage';
import { AuthProvider } from './Components/AuthContext';
import ProtectedRoute from './Components/ProtectedRoute';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/" element={<NavBar />}>
            <Route element={<ProtectedRoute />}>
              <Route path="/" element={<Home />} />
              <Route path="/Users" element={<UsersPage />} />
              <Route path="/Items" element={<ItemsPage />} />
              <Route path="/Recover" element={<Recover />} />
            </Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
export default App;

