from flask_jwt_extended import jwt_required, get_jwt_identity
from flask import Blueprint, request, jsonify
from service.inventory_service import update_items, get_char_inventory, get_all_items

inventory_route = Blueprint('inventory_route', __name__)


@inventory_route.route('/update', methods=['POST'])
def update_items_route():
    data = request.get_json()
    update_items(data.get('array', []))
    return '', 200


@inventory_route.route('/get', methods=['GET'])
@jwt_required()
def get_inventory():
    char_id = request.headers.get('CharID')
    if not char_id:
        return jsonify({"error": "CharID missing"}), 400
    return get_char_inventory(char_id)


# ##### ADD AUTHORIZATION!
@inventory_route.route('/<char_id>', methods=['GET'])
def get_character_by_id(char_id):
    return get_char_inventory(char_id)


@inventory_route.route('/items', methods=['GET'])
def get_all_items_route():
    return get_all_items()

