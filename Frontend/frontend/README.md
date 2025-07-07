# Python Project - Frontend

# Authors

name: "Ilya Kirshtein",

email: "ilyaki@edu.jmc.ac.il"


name: "Dima Nikonov"

email: "dimaku@edu.jmc.ac.il"

---

## How to Run:

`npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

Login with username: admin, password: admin.

Make sure to have the backend flask running, see root folder README.

---

## Features

### Users Page

- See all users.
- Download gameplay statistics.
- See and edit users characters and their inventories.

### Items Page

- View a list of items available in game.

### Recover Page

- View banned users and deleted characters.
- Restore banned users, and deleted characters.

---

## Project Structure

### Main Components

- **Home.jsx**  
  An introduction page, with a short use guide, and authors data.

- **UserPage.jsx**  
  Displays the list users, with 3 buttons as described in features.

- **UserInfo.jsx**  
  Displays more info about a user, such as their characters and inventories.

- **ItemsAccordion.jsx**  
  A bootstrap accordion to simulate the inventory, **The slots are clickable**.

- **ItemsModal.jsx**  
  A bootstrap modal to edit an inventory slot.

- **ItemsPage.jsx**  
  Displays a list of items available in game.

- **Recover.jsx**  
  Displays banned users and deleted characters.

- **AuthContext.jsx && ProtectedRoute.jsx**
  Utility wrappers to handle admin authentication and getting access for connection with backend.


### Utilities

- **DropDown.jsx**  
  Configurable Bootstrap dropdown with customizable options and callbacks.

- **NavBar.jsx**  
  Simulated multi-page routing using `react-router`.

- **About.jsx**  
  A Bootstrap card showing information about each author.

- **AlertComponent.jsx**  
  A Bootstrap alert component with customizable error massages.

- **IconButton.jsx**
  A Configurable Bootstrap button with ability too add bootstrap icons.

### Custom Hooks

- **useAPI.jsx**  
  Custom React hook for fetching data from the backend (GET only).

---


