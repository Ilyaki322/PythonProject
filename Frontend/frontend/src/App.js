import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';

import { BrowserRouter, Route, Routes } from "react-router";
import NavBar from "./Components/NavBar";
import UsersPage from './Components/UsersPage';
import ItemsPage from './Components/ItemsPage';
import Recover from './Components/Recover';
import Home from './Components/Home';

function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<NavBar />}>
          <Route path="/" element={<Home />} />
          <Route path="/Users" element={<UsersPage />} />
          <Route path="/Items" element={<ItemsPage />} />
          <Route path="/Recover" element={<Recover />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );

}
export default App;

