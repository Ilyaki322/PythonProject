# Full Stack Multiplayer Game Project

# Authors

name: "Ilya Kirshtein",

email: "ilyaki@edu.jmc.ac.il"


name: "Dima Nikonov"

email: "dimaku@edu.jmc.ac.il"

---

## Overview

This project consists of three main components:

Unity(C#) - A multiplayer game where players can create characters, queue for matches, and compete.

Flask(Python) - Backend REST API and socket server for handling gameplay logic, authentication, and data.

React(JavaScript) - An admin dashboard to manage and visualize game data.

## Game Features:

* User Login and Registration via email/password or Google OAuth.

* Character creation with a customization system.

* Matchmaking queue to find and battle other players in real-time.

* Rewards system with leveling, item purchasing, and progression.

* Multiplayer support using Sockets and Flask Socket.IO.

## Backend Features:

* REST API protected by JWT tokens for secure game logic and user authentication.

* Admin-only API for modifying users, characters, items, and other in-game data.

* WebSocket (Socket.IO) server to handle real-time gameplay events.

* PostgreSQL as the primary database.

## Frontend Features:

* View all game related data, like players, characters, item info

* Download Excel reports that show player stats and analytics.

* Perform admin actions like: Banning/unbanning players, editing charcater stats and manage character inventories!

---

## Requirements

Python 3.12+ and an enviroment to run it (like pycharm).

Docker (optional but recommended) to run PostgreSQL

PostgreSQL (if not using Docker)

Node.js and npm for the React frontend

Unity 6 is **not** required, a build for windows / mac is provided

Both React and Flask use external libraries that needs installing,
details in each individual README

---

## How to Run

Backend Setup:

1. Clone the repository, then open the flask-backend/ folder in PyCharm or your preferred IDE.

2. Install dependencies: pip install -r requirements.txt

3. Start the Flask app: python app.py

Database Setup with Docker: 

docker run -d -e POSTGRES_DB=pyproj -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=Aa123456 -p 5433:5432 postgres:latest

Frontend Setup:

1. Install dependencies: npm install

2. Run: npm start

Game Client:

Run the provided Windows/Mac build in the build/ folder.

Open the Unity project if you have Unity 6 installed.

**Note: to simulate multiplayer run the executable twice, or use the Unity built in feature,

At the top of the screen:

Window --> Multiplayer --> Multiplayer Play Mode**








