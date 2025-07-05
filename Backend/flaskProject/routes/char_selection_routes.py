from flask_jwt_extended import jwt_required, get_jwt_identity
from flask import Blueprint, request, jsonify
from service.character_service import get_characters as gt
from service.character_service import add_character as ac

selection_route = Blueprint('selection_route', __name__)


@selection_route.route('/', methods=['GET'])
@jwt_required()
def get_characters():
    user_id = get_jwt_identity()
    return gt(user_id)


@selection_route.route('/add', methods=['POST'])
@jwt_required()
def add_char():
    user_id = get_jwt_identity()
    data = request.get_json()
    return ac(data, user_id)


# ##### ADD AUTHORIZATION!
@selection_route.route('/<user_id>', methods=['GET'])
def get_character_by_id(user_id):
    return gt(user_id)


