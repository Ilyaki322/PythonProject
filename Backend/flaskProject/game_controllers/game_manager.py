from flask_socketio import SocketIO, send, emit


class GameManager:
    def __init__(self, player1_sid, player2_sid, is_player1_turn, socketio):
        self.player1_sid = player1_sid
        self.player2_sid = player2_sid
        self.is_player1_turn = is_player1_turn
        self.socketio = socketio

    def has_player(self, sid):
        return sid == self.player1_sid or sid == self.player2_sid

    def on_attack(self, user_sid):
        dest = self.player2_sid if self.is_player1_turn else self.player1_sid
        self.is_player1_turn = not self.is_player1_turn
        emit("Attack", to=dest)

    def on_defend(self, user_sid):
        dest = self.player2_sid if self.is_player1_turn else self.player1_sid
        self.is_player1_turn = not self.is_player1_turn
        emit("Defend", to=dest)

    def on_use_item(self, user_sid):
        dest = self.player2_sid if self.is_player1_turn else self.player1_sid
        self.is_player1_turn = not self.is_player1_turn
        emit("UseItem", to=dest)

    def on_end_game(self, user_sid):
        loser = user_sid
        winner = self.player1_sid if user_sid == self.player2_sid else self.player2_sid
        emit("Win", to=winner)
        emit("Lose", to=loser)

