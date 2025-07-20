## Python backend Structure

The project is structured to maintain clear separation of concerns, with each folder and file serving a specific purpose. Below is a general description of the main components of the project:

1. **errors/**  
   Contains custom exception classes and request‑validation logic.

2. **game_controllers/**  
   Implements game flow controllers for matchmaking, turn management, and overall game state.

3. **models/**  
   Defines SQLAlchemy ORM models mapping to database tables for characters, users, items, matches, etc.

4. **routes/**  
   Houses Flask Blueprints and route handlers for REST endpoints and Socket.IO events.

5. **service/**  
   Encapsulates business logic and data‑access operations invoked by the controllers.

6. **config.py**  
   Contains configuration classes (e.g., development, production) and environment variable handling.

7. **db.py**  
   Initializes and configures Flask extensions (SQLAlchemy, Bcrypt).

8. **app.py**  
   Application factory and entry point that sets up Flask, extensions, and registers blueprints.

9. **validators/** *(optional grouping)*  
   Utility functions for validating incoming request data.

## REST API Endpoints

* Character Selection (char_selection_routes.py)
Exposes CRUD operations for player avatars, including listing active or deleted characters (GET / & /deleted), creating new characters (POST /add), editing (PATCH /update_character), deleting (DELETE /delete_character), and recovering (PATCH /recover_character). All routes require JWT authentication and return consistent JSON responses with appropriate status codes.


* Inventory Management (inventory_routes.py)
Handles batch imports of item definitions (POST /update), retrieval of a character’s inventory via header or URL param (GET /get & GET /<char_id>), listing all game items (GET /items), and fine‑grained slot updates (PUT /update_slot). Validation of payloads is enforced with custom validators, and responses follow REST conventions.


* Authentication & User Management (login_routes.py)
Manages user login (POST /login), admin login (POST /login_admin), and registration (POST /register) without JWT, then protects user listing (GET /users, GET /deleted_users), soft‑deletion (PATCH /delete), and recovery (PATCH /recover) behind JWT. A dedicated endpoint (GET /user_data/<user_id>) streams an Excel report of match history.


* OAuth2 via Google (oauth_routes.py)
Orchestrates the OAuth2 handshake: redirecting to Google’s consent page (GET /login/google), handling the callback to exchange code for tokens and issuing a JWT (GET /login/google/callback), and polling for login status (GET /login/status). Uses secure state tokens to prevent CSRF and returns HTML for in‑client flows.

## Socket.IO Events

* Connection Lifecycle (socket_routes.py)
On connect, validates the client’s JWT query token and registers the socket ID with the matchmaking controller; on disconnect, removes the player. Emits a connected acknowledgement upon successful handshake.


* Matchmaking (socket_routes.py)
EnterQueue adds a player to the matchmaking queue and, when paired, emits MatchFound to both participants with their own and the enemy’s character data plus turn order. LeaveQueue allows exit before pairing.


* Gameplay Actions (socket_routes.py & game_controllers)
Handles turn‑based combat events—Attack, Defend, SkipTurn, UseItem—by delegating to GameManager methods that flip turns and broadcast actions to the opponent. EndGame finalizes the match, awards gold, persists results, and emits Win/Lose.


* Shop Interactions (shop_socket_routes.py)
Listens for LevelUp, PurchaseItem, SellItem, and SwapSlots events to update character stats and inventory via service‑layer functions. On successful operations automatically emit any necessary confirmations back to the client.
