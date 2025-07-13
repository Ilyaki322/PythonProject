from flask import Blueprint, request, jsonify
from flask_jwt_extended import decode_token
from flask_socketio import SocketIO, send, emit
from game_controllers.game_manager import GameManager
from service.character_service import *
import random

_app = None
_socketio = None

connected_players = {}
players_in_queue = {}
ongoing_matches = []


def init_socket_handlers(app_instance, socketio_instance):
    global _app, _socketio, connected_players
    _app = app_instance
    _socketio = socketio_instance

    @_socketio.on('connect')
    def handle_connect():
        token = request.args.get('token')
        if not token:
            print("No token provided, rejecting")
            return False

        try:
            decoded_token = decode_token(token)
            print(f"Welcome User: {decoded_token['sub']}")
            connected_players[request.sid] = decoded_token['sub']
        except Exception as e:
            print(f"Invalid token: {e}")
            return False

        emit('connected', {'msg': 'Welcome Unity!'})

    @_socketio.on('disconnect')
    def handle_disconnect():
        sid = request.sid
        user_id = connected_players.pop(sid, None)
        print(f"User {user_id} disconnected. SID: {sid}")

    @_socketio.on('EnterQueue')
    def handle_enter_queue(data):
        # !!!!!
        # check that user cant queue twice, if you queue, reconnect get other sid and queue,
        # you fight yourself.
        # !!!!!
        char_id = data.get('charID')
        players_in_queue[request.sid] = char_id
        print(f"Player entered queue with char: {char_id}")
        if len(players_in_queue) >= 2:
            players = list(players_in_queue.items())[:2]
            players_in_queue.pop(players[0][0])
            players_in_queue.pop(players[1][0])
            start_match(players[0], players[1])

    @_socketio.on('LeaveQueue')
    def handle_leave_queue():
        user_sid = request.sid
        if user_sid:
            removed = players_in_queue.pop(user_sid, None)
            print(f"Player {user_sid} left queue. Removed char: {removed}")
        else:
            print("SID not found in connected_players")

    @_socketio.on('Attack')
    def handle_attack():
        user_sid = request.sid
        game = find_match(user_sid)
        if game:
            game.on_attack(user_sid)
        else:
            print("ERROR, USER NOT IN MATCH")

    @_socketio.on('Defend')
    def handle_attack():
        user_sid = request.sid
        game = find_match(user_sid)
        if game:
            game.on_defend(user_sid)
        else:
            print("ERROR, USER NOT IN MATCH")

    @_socketio.on('UseItem')
    def handle_attack():
        user_sid = request.sid
        game = find_match(user_sid)
        if game:
            game.on_use_item(user_sid)
        else:
            print("ERROR, USER NOT IN MATCH")

    @_socketio.on('EndGame')
    def handle_attack():
        user_sid = request.sid
        game = find_match(user_sid)
        if game:
            game.on_end_game(user_sid)
        else:
            print("ERROR, USER NOT IN MATCH")
        ongoing_matches.remove(game)

    @_socketio.on('SkipTurn')
    def handle_skip():
        user_sid = request.sid
        game = find_match(user_sid)
        if game:
            game.on_skip_turn(user_sid)
        else:
            print("ERROR, USER NOT IN MATCH")


def start_match(player1, player2):
    print("MATCH FOUND!")
    char1 = get_char_by_id(player1[1])
    char2 = get_char_by_id(player2[1])

    player1_starts = random.choice([True, False])
    json1 = {"player_character": char1, "enemy_character": char2, "is_starting": player1_starts}
    json2 = {"player_character": char2, "enemy_character": char1, "is_starting": not player1_starts}

    ongoing_matches.append(GameManager(player1[0], player2[0], player1_starts,
                                       connected_players[player1[0]], connected_players[player2[0]]))

    emit("MatchFound", json1, to=player1[0])
    emit("MatchFound", json2, to=player2[0])


def find_match(user_sid):
    for game in ongoing_matches:
        if game.has_player(user_sid):
            return game














