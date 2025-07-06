import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';

import { BrowserRouter, Route, Routes } from "react-router";
import NavBar from "./Components/NavBar";
import UsersPage from './Components/UsersPage';
import About from './Components/About';
import ItemsPage from './Components/ItemsPage';

function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<NavBar />}>
          <Route path="/" element={<UsersPage />} />
          <Route path="/Items" element={<ItemsPage />} />
          <Route path="/About" element={<About />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );

}
export default App;

