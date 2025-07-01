from flask_jwt_extended import jwt_required, get_jwt_identity
from flask import Blueprint, request, jsonify
from service.inventory_service import update_items, get_char_inventory

inventory_route = Blueprint('inventory_route', __name__)


@inventory_route.route('/update', methods=['POST'])
def update_items_route():
    data = request.get_json()
    update_items(data.get('array', []))
    return '', 200


@inventory_route.route('/get', methods=['GET'])
@jwt_required
def get_inventory():
    data = request.get_json()
    return get_char_inventory(data.get("char_id"))

