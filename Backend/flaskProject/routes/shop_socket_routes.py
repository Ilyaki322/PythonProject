# shop_socket_handlers.py
import requests
from flask import request
from flask_socketio import emit
from service.character_service import update_character_gold, character_levelUp
import service.inventory_service as inventory_service

_app = None
_socketio = None


def init_shop_socket_handlers(app_instance, socketio_instance):
    global _app, _socketio
    _app = app_instance
    _socketio = socketio_instance

    @_socketio.on('LevelUp')
    def handle_level_up(data):
        """
        data = {'charId'     : int, 'newLevel'   : int, 'currentGold': int}
        """
        char_id = data.get('charId')
        new_level = data.get('newLevel')
        current_gold = data.get('currentGold')

        if char_id is None or new_level is None or current_gold is None:
            print("[Shop] LevelUp missing fields:", data)
            return

        try:
            character_levelUp(char_id, new_level, current_gold)
        except Exception as e:
            print(f"[Shop] LevelUp error: {e}")

    @_socketio.on('PurchaseItem')
    def handle_purchase(data):
        """
        data: {'character_id', 'index', 'item_id', 'count'}
        """

        payload = {'character_id': data['charId'], 'index': data['slotIndex'],
                   'item_id': data['itemId'], 'count': data['count']}

        try:
            service_response = inventory_service.buy_item(payload)
            if service_response.get('success'):
                update_character_gold(data.get('charId'), data.get('currentGold'))

        except Exception as e:
            print(f"[Shop] PurchaseItem request error: {e}")

    @_socketio.on('SellItem')
    def handle_sale(data):
        """
        data: { 'charId': int, 'itemId': int, 'qty': int, 'slotIndex': int, currentGold': int' }
        """
        payload = {'character_id': data['charId'], 'index': data['slotIndex'],
                   'item_id': data['itemId'], 'count': data['count']}

        try:
            service_response = inventory_service.sell_item(payload)
            if service_response.get('success'):
                update_character_gold(data.get('charId'), data.get('currentGold'))

        except Exception as e:
            print(f"[Shop] PurchaseItem request error: {e}")

    @_socketio.on('SwapSlots')
    def handle_swap(data):
        """
        data: {'characterId': int, 'indexSrc': int, 'indexDest': int}
        """
        payload = {'character_id': data['charId'], 'indexSrc': data['indexSrc'], 'indexDest': data['indexDest']}

        try:
            inventory_service.swap_items(payload)
        except Exception as e:
            print(f"[Shop] PurchaseItem request error: {e}")
