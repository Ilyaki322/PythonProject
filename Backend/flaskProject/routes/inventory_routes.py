from flask_jwt_extended import jwt_required, get_jwt_identity
from flask import Blueprint, request, jsonify
from service.inventory_service import update_items, get_char_inventory, get_all_items, update_slot
from errors.validators import *

"""
inventory_route module

Defines endpoints for managing and retrieving inventory data,
including batch updates, character‑specific inventories, and item listings.
"""

inventory_route = Blueprint('inventory_route', __name__)


@inventory_route.route('/update', methods=['POST'])
def update_items_route():
    """
        POST /update

        Batch import or update multiple items in the database.
        No authentication enforced; suitable for initial data sync.

        Body JSON:
            { "array": list[dict] }

        Returns:
            '' (empty body), 200
        """

    data = request.get_json()
    update_items(data.get('array', []))
    return '', 200


@inventory_route.route('/get', methods=['GET'])
@jwt_required()
def get_inventory():
    """
        GET /get

        Retrieve the inventory for the current character.

        Headers:
            CharID (str): ID of the character whose inventory is fetched.

        Returns:
            JSON list of inventory entries, 200
        """
    char_id = request.headers.get('CharID')
    print(char_id)
    if not char_id:
        raise BadRequest(f"Missing fields: CharID")

    return jsonify(get_char_inventory(char_id)), 200


@inventory_route.route('/<char_id>', methods=['GET'])
@jwt_required()
def get_character_by_id(char_id):
    """
        GET /<char_id>

        Fetch inventory entries for the specified character ID.

        Path Params:
            char_id (str): Target character’s ID.

        Returns:
            JSON list of inventory entries, 200
        """
    return jsonify(get_char_inventory(char_id)), 200


@inventory_route.route('/items', methods=['GET'])
@jwt_required()
def get_all_items_route():
    """
        GET /items

        List all items available in the game.

        Returns:
            JSON list of all item dicts, 200
        """
    return jsonify(get_all_items()), 200


@inventory_route.route('/update_slot', methods=['PUT'])
@jwt_required()
def update_slot_route():
    """
        PUT /update_slot

        Modify a single inventory slot for a character.

        Body JSON:
            {
                "character_id": int,
                "index": int,
                "item_id": str,
                "count": int
            }

        Returns:
            JSON response from service, status code
        """
    data = request.get_json()
    validate_slot_update(data)

    response, code = update_slot(data)
    return jsonify(response), code
