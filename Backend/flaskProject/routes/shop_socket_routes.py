from service.character_service import update_character_gold, character_levelUp
import service.inventory_service as inventory_service

_app = None
_socketio = None

"""
shop_socket_handlers module

Initializes and registers Socket.IO event handlers under the shop
namespace. Handles level‑up, purchase, sale, and slot swap events,
invoking service‑layer functions and emitting results to clients.
"""


def init_shop_socket_handlers(app_instance, socketio_instance):
    """
        Configure Flask‑SocketIO handlers for shop events.

        Args:
            app_instance: The Flask app object.
            socketio_instance: The Flask‑SocketIO instance.
        """
    global _app, _socketio
    _app = app_instance
    _socketio = socketio_instance

    @_socketio.on('LevelUp')
    def handle_level_up(data):
        """
               LevelUp handler

               Expected data:
                   {
                       "charId": int,
                       "newLevel": int,
                       "currentGold": int
                   }
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
                PurchaseItem handler

                Expected data:
                    {
                        "charId": int,
                        "itemId": int,
                        "count": int,
                        "slotIndex": int,
                        "cost": int
                    }
        """

        payload = {'character_id': data['charId'], 'index': data['slotIndex'],
                   'item_id': data['itemId'], 'count': data['count']}

        try:
            service_response = inventory_service.buy_item(payload)
            if service_response.get('success'):
                update_character_gold(data.get('charId'), -data.get('cost'))

        except Exception as e:
            print(f"[Shop] PurchaseItem request error: {e}")

    @_socketio.on('SellItem')
    def handle_sale(data):
        """
        SellItem handler

        Expected data:
            {
                "charId": int,
                "itemId": int,
                "count": int,
                "slotIndex": int,
                "cost": int
            }
        """
        payload = {'character_id': data['charId'], 'index': data['slotIndex'],
                   'item_id': data['itemId'], 'count': data['count']}

        try:
            service_response = inventory_service.sell_item(payload)
            if service_response.get('success'):
                update_character_gold(data.get('charId'), data.get('cost'))

        except Exception as e:
            print(f"[Shop] PurchaseItem request error: {e}")

    @_socketio.on('SwapSlots')
    def handle_swap(data):
        """
        SwapSlots handler

        Expected data:
            {
                "charId": int,
                "indexSrc": int,
                "indexDest": int
            }
        """
        payload = {'character_id': data['charId'], 'indexSrc': data['indexSrc'], 'indexDest': data['indexDest']}

        try:
            inventory_service.swap_items(payload)
        except Exception as e:
            print(f"[Shop] PurchaseItem request error: {e}")
