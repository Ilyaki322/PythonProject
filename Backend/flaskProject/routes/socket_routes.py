from flask import Blueprint, request, jsonify
from flask_jwt_extended import decode_token
from flask_socketio import SocketIO, send, emit

_app = None
_socketio = None

connected_players = {}
players_in_queue = {}


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
        char_id = data.get('charID')
        players_in_queue[connected_players[request.sid]] = char_id
        print(f"Player entered queue with char: {char_id}")
        if len(players_in_queue) >= 2:
            print("MATCH FOUND!")

    @_socketio.on('LeaveQueue')
    def handle_leave_queue(data):
        user_id = connected_players.get(request.sid)
        if user_id:
            removed = players_in_queue.pop(user_id, None)
            print(f"Player {user_id} left queue. Removed char: {removed}")
        else:
            print("SID not found in connected_players")
