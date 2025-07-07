from flask_jwt_extended import jwt_required, get_jwt_identity
from flask import Blueprint, request, jsonify
from service.inventory_service import update_items, get_char_inventory, get_all_items, update_slot
from errors.validators import *

inventory_route = Blueprint('inventory_route', __name__)


# UTILITY FUNCTION TO SAVE ALL ITEMS IN UNITY TO DB
# NOT SURE ABOUT AUTH
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
        raise BadRequest(f"Missing fields: CharID")

    return jsonify(get_char_inventory(char_id)), 200


@inventory_route.route('/<char_id>', methods=['GET'])
@jwt_required()
def get_character_by_id(char_id):
    return jsonify(get_char_inventory(char_id)), 200


@inventory_route.route('/items', methods=['GET'])
@jwt_required()
def get_all_items_route():
    return jsonify(get_all_items()), 200


@inventory_route.route('/update_slot', methods=['PUT'])
@jwt_required()
def update_slot_route():
    data = request.get_json()
    validate_slot_update(data)

    response, code = update_slot(data)
    return jsonify(response), code
