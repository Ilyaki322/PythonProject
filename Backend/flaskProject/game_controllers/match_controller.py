from game_controllers.game_manager import GameManager
from service.character_service import get_char_by_id
import random

"""
MatchController module
Singleton class,
Manages players’ connections, matchmaking queue, and active GameManager instances
for turn‑based matches.
"""


class MatchController:
    """
        Singleton controller for matching players into games.

        Attributes:
            connected_players (dict): sid → user_id mapping of online players.
            players_in_queue (dict): sid → character_id mapping of players waiting for match.
            ongoing_matches (list): Active GameManager instances.
        """
    _instance = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(MatchController, cls).__new__(cls)
            cls._instance.connected_players = {}
            cls._instance.players_in_queue = {}
            cls._instance.ongoing_matches = []

        return cls._instance

    def add_connected_player(self, sid, user_id):
        """Register a newly connected player by their socket ID."""
        self.connected_players[sid] = user_id

    def disconnect_player(self, sid):
        """Remove a player from connected list when they disconnect."""
        return self.connected_players.pop(sid, None)

    def enter_queue(self, sid, char_id):
        """
                Add player to matchmaking queue.
                If two players are waiting, pop them both and start a match.
                Returns (game, dto1, dto2, first_turn) or None.
                """
        print(f"{sid} charId {char_id} entered QUEUE")
        if sid in self.players_in_queue:
            return None

        self.players_in_queue[sid] = char_id

        if len(self.players_in_queue) >= 2:
            (sid1, cid1), (sid2, cid2) = list(self.players_in_queue.items())[:2]

            self.players_in_queue.pop(sid1)
            self.players_in_queue.pop(sid2)
            return self._start_match((sid1, cid1), (sid2, cid2))

        return None

    def leave_queue(self, sid):
        """Allow a player to exit the matchmaking queue."""
        return self.players_in_queue.pop(sid, None)

    def _start_match(self, player1, player2):
        """
                Instantiate GameManager for two queued players.
                Returns the GameManager, both character DTOs, and who starts.
                """
        sid1, db1 = player1
        sid2, db2 = player2
        print(f"starting match player1: {sid1} {db1} player2 {sid2} {db2}")
        dto1 = get_char_by_id(db1)
        dto2 = get_char_by_id(db2)
        first_turn = random.choice([True, False])

        game = GameManager(
            player1_sid=sid1,
            player2_sid=sid2,
            is_player1_turn=first_turn,
            player1_id=self.connected_players[sid1],
            player2_id=self.connected_players[sid2],
            player1_char_id=db1,
            player2_char_id=db2
        )
        self.ongoing_matches.append(game)
        return game, dto1, dto2, first_turn

    def find_match(self, sid):
        """Locate the active GameManager for a given player socket ID."""
        for game in self.ongoing_matches:
            if game.has_player(sid):
                return game

    def end_game(self, game):
        """Clean up after a match ends by removing it from ongoing_matches."""
        try:
            self.ongoing_matches.remove(game)
        except ValueError:
            pass
