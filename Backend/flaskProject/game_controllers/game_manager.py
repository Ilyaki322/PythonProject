from flask_socketio import emit
from db import db
from models.match import Match
from datetime import datetime, UTC
from service.character_service import update_character_gold as upd_gold

"""
GameManager module

This module defines the GameManager class, which orchestrates a turn‑based
match between two players connected via Flask‑SocketIO. It handles real‑time
events (attack, defend, skip, use item), manages turn switching, updates
player gold, persists match results to the database, and emits outcome
events to the clients.
"""


class GameManager:
    """
        Manages a two‑player, turn‑based game session.

        Attributes:
            player1_sid (str): Socket ID for player1.
            player2_sid (str): Socket ID for player2.
            is_player1_turn (bool): True if it is currently player1’s turn.
            player1_id (int): Database user ID for player1.
            player2_id (int): Database user ID for player2.
            player1_char_id (int): Character ID for player1.
            player2_char_id (int): Character ID for player2.
        """
    def __init__(self, player1_sid, player2_sid,
                 is_player1_turn, player1_id, player2_id,
                 player1_char_id, player2_char_id):
        self.player1_sid = player1_sid
        self.player2_sid = player2_sid
        self.is_player1_turn = is_player1_turn
        self.player1_id = player1_id
        self.player2_id = player2_id
        self.player1_char_id = player1_char_id
        self.player2_char_id = player2_char_id

    def has_player(self, sid):
        return sid == self.player1_sid or sid == self.player2_sid

    def on_attack(self, user_sid, damage):
        dest = self.get_next_turn_player()
        self.is_player1_turn = not self.is_player1_turn
        emit("Attack", damage, to=dest)

    def on_defend(self, user_sid):
        dest = self.get_next_turn_player()
        self.is_player1_turn = not self.is_player1_turn
        emit("Defend", to=dest)

    def on_skip_turn(self, user_sid):
        dest = self.get_next_turn_player()
        self.is_player1_turn = not self.is_player1_turn
        emit("SkipTurn", to=dest)

    def on_use_item(self, user_sid, item_guid):
        dest = self.get_next_turn_player()
        emit("UseItem", item_guid, to=dest)

    def on_end_game(self, user_sid):
        loser = user_sid
        winner = self.player1_sid if user_sid == self.player2_sid else self.player2_sid

        if winner == self.player1_sid:
            upd_gold(self.player1_char_id, 3)
            upd_gold(self.player2_char_id, 1)
        else:
            upd_gold(self.player1_char_id, 1)
            upd_gold(self.player2_char_id, 3)

        winner_id = self.player1_id if winner == self.player1_sid else self.player2_id

        # Create Match entry
        new_match = Match(
            match_start=datetime.now(UTC),
            player_1_id=self.player1_id,
            player_2_id=self.player2_id,
            winner_id=winner_id
        )
        db.session.add(new_match)
        db.session.commit()

        emit("Win", to=winner)
        emit("Lose", to=loser)

    def get_current_turn_player(self):
        return self.player1_sid if self.is_player1_turn else self.player2_sid

    def get_next_turn_player(self):
        return self.player2_sid if self.is_player1_turn else self.player1_sid
