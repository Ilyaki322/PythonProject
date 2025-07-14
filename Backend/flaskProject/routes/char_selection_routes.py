from flask_jwt_extended import jwt_required, get_jwt_identity
from flask import Blueprint, request, jsonify
from service.character_service import *
from errors.validators import *

selection_route = Blueprint('selection_route', __name__)


@selection_route.route('/', methods=['GET'])
@jwt_required()
def get_characters_route():
    user_id = get_jwt_identity()
    characters = get_characters(user_id, deleted=False)
    return jsonify({
        "count": len(characters),
        "characters": characters
    }), 200


@selection_route.route('/add', methods=['POST'])
@jwt_required()
def add_char_route():
    user_id = get_jwt_identity()
    data = request.get_json()
    validate_character_data(data)
    return jsonify(add_character(data, user_id)), 201


@selection_route.route('/<user_id>', methods=['GET'])
@jwt_required()
def get_character_by_id(user_id):
    characters = get_characters(user_id, deleted=False)
    return jsonify({
        "count": len(characters),
        "characters": characters
    }), 200


@selection_route.route('/deleted', methods=['GET'])
@jwt_required()
def get_character_by_id_deleted():
    character_data = get_characters_deleted()
    return jsonify({"characters": character_data}), 200


@selection_route.route('/update_character', methods=['PATCH'])
@jwt_required()
def update_character_route():
    if get_jwt_identity() != "ADMIN":
        return jsonify({'error': 'Forbidden'}), 403

    data = request.get_json()
    validate_character_edit_data(data)
    return jsonify(edit_character(data)), 200


@selection_route.route('/delete_character', methods=['DELETE'])
@jwt_required()
def delete_character_route():
    data = request.get_json()
    validate_character_delete_data(data)
    char = set_delete_character(data, True)
    return jsonify(char.to_dict()), 200


@selection_route.route('/recover_character', methods=['PATCH'])
@jwt_required()
def recover_character_route():
    data = request.get_json()
    validate_character_delete_data(data)
    char = set_delete_character(data, False)
    return jsonify(char.to_dict()), 200

