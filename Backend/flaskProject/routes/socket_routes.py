from flask import request
from flask_jwt_extended import decode_token
from flask_socketio import emit
from service.inventory_service import *
from routes.shop_socket_routes import init_shop_socket_handlers
from game_controllers.match_controller import MatchController

_app = None
_socketio = None

"""
socket_routes module

Defines Socket.IO handlers for:
- User connect/disconnect and matchmaking with MatchController
- Shop events (level-up, purchase, sale, swap)
Uses Flask-SocketIO for real‑time communication and integrates
with service layers for business logic.
"""

controller = MatchController()


def init_socket_handlers(app_instance, socketio_instance):
    """
        Configure application and register all Socket.IO event handlers.

        Args:
            app_instance: Flask application instance.
            socketio_instance: Flask-SocketIO instance.
        """
    global _app, _socketio
    _app = app_instance
    _socketio = socketio_instance

    init_shop_socket_handlers(app_instance, socketio_instance)

    @_socketio.on('connect')
    def handle_connect():
        """
            connect handler

            Expects:
                Query param 'token': JWT for user authentication.

            Actions:
                - Validates token via `decode_token`.
                - Registers socket ID with MatchController.
                - Emits 'connected' on success, or rejects connection.
            """
        token = request.args.get('token')
        if not token:
            return False
        try:
            decoded_token = decode_token(token)
            user_id = decoded_token['sub']
            controller.add_connected_player(request.sid, user_id)
        except Exception as e:
            return False
        emit('connected', {'msg': 'Welcome Unity!'})

    @_socketio.on('disconnect')
    def handle_disconnect():
        """
            disconnect handler

            Invoked when a client disconnects; removes player from controller.
            """
        controller.disconnect_player(request.sid)

    @_socketio.on('EnterQueue')
    def handle_enter_queue(data):
        """
            EnterQueue handler

            Expected data:
                { "charID": int }

            Actions:
                - Adds player to matchmaking queue.
                - When two players queue, starts a match and emits 'MatchFound'.
            """
        char_id = data.get('charID')
        match = controller.enter_queue(request.sid, char_id)
        if not match:
            return

        game, dto1, dto2, first_turn = match
        sid1, sid2 = game.player1_sid, game.player2_sid

        emit("MatchFound", {
            "player_character": dto1,
            "enemy_character": dto2,
            "is_starting": first_turn
        }, to=sid1)

        emit("MatchFound", {
            "player_character": dto2,
            "enemy_character": dto1,
            "is_starting": not first_turn
        }, to=sid2)

    @_socketio.on('LeaveQueue')
    def handle_leave_queue():
        """
            LeaveQueue handler

            Removes player from matchmaking queue.
            """
        controller.leave_queue(request.sid)

    @_socketio.on('Attack')
    def handle_attack(data):
        """
            Attack handler

            Expected data:
                { "damageDealt": int }

            Delegates to GameManager.on_attack and emits to next player.
            """
        game = controller.find_match(request.sid)
        if game:
            game.on_attack(request.sid, data.get('damageDealt'))
        else:
            return

    @_socketio.on('Defend')
    def handle_defend():
        """
            Defend handler

            Delegates to GameManager.on_defend and emits to next player.
            """
        game = controller.find_match(request.sid)
        if game:
            game.on_defend(request.sid)
        else:
            return

    @_socketio.on('UseItem')
    def handle_use_item(data):
        """
            UseItem handler

            Expected data:
                {
                    "index": int,
                    "itemId": <any>
                }

            Actions:
                - Updates inventory slot via `update_slot`.
                - Delegates to GameManager.on_use_item and emits.
            """
        user_sid = request.sid
        char_id = controller.connected_players.get(user_sid)

        if not char_id:
            return

        """ character_id, item_id, count, index """
        update_slot({'character_id': char_id, 'index': data.get('index'), 'count': 0, 'item_id': None})

        game = controller.find_match(user_sid)
        if game:
            game.on_use_item(user_sid, data.get('itemId'))
        else:
            return

    @_socketio.on('EndGame')
    def handle_attack():
        """
            EndGame handler

            Delegates to GameManager.on_end_game, persists match, and cleans up.
            """
        user_sid = request.sid
        game = controller.find_match(user_sid)
        if game:
            game.on_end_game(user_sid)
            controller.end_game(game)
        else:
            return

    @_socketio.on('SkipTurn')
    def handle_skip():
        """
            SkipTurn handler

            Delegates to GameManager.on_skip_turn and emits to next player.
            """
        game = controller.find_match(request.sid)
        if game:
            game.on_skip_turn(request.sid)
        else:
            return
