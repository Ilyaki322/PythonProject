from flask_jwt_extended import jwt_required, get_jwt_identity
from flask import Blueprint, request, jsonify
from service.character_service import *
from errors.validators import *

"""
selection_route module

Defines the character selection and management endpoints under a Flask Blueprint.
All routes require a valid JWT and handle JSON payloads for listing, adding,
updating, deleting, and recovering characters.
"""

selection_route = Blueprint('selection_route', __name__)


@selection_route.route('/', methods=['GET'])
@jwt_required()
def get_characters_route():
    """
        GET /

        Retrieve all non‑deleted characters for the current user.

        Returns:
            JSON { "count": int, "characters": list[dict] }, 200
        """
    user_id = get_jwt_identity()
    characters = get_characters(user_id, deleted=False)
    return jsonify({
        "count": len(characters),
        "characters": characters
    }), 200


@selection_route.route('/add', methods=['POST'])
@jwt_required()
def add_char_route():
    """
        POST /add

        Create a new character for the current user.

        Body JSON:
            {
                "name": str,
                "level": int,
                "hair": int,
                "helmet": int,
                "beard": int,
                "armor": int,
                "pants": int,
                "weapon": int,
                "back": int
            }

        Returns:
            JSON representation of the new character, 201
        """
    user_id = get_jwt_identity()
    data = request.get_json()
    validate_character_data(data)
    return jsonify(add_character(data, user_id)), 201


@selection_route.route('/<user_id>', methods=['GET'])
@jwt_required()
def get_character_by_id(user_id):
    """
        GET /<user_id>

        List all non‑deleted characters for a specified user ID.

        Args:
            user_id (str): Target user’s ID in the URL path.

        Returns:
            JSON { "count": int, "characters": list[dict] }, 200
        """
    characters = get_characters(user_id, deleted=False)
    return jsonify({
        "count": len(characters),
        "characters": characters
    }), 200


@selection_route.route('/deleted', methods=['GET'])
@jwt_required()
def get_character_by_id_deleted():
    """
        GET /deleted

        Retrieve all characters that have been soft‑deleted across all users.

        Returns:
            JSON { "characters": list[dict] }, 200
        """
    character_data = get_characters_deleted()
    return jsonify({"characters": character_data}), 200


@selection_route.route('/update_character', methods=['PATCH'])
@jwt_required()
def update_character_route():
    """
        PATCH /update_character

        Update an existing character’s name or level.
        Only accessible to ADMIN users.

        Body JSON:
            {
                "character_id": int,
                "name": str,
                "level": int
            }

        Returns:
            JSON of the updated character, 200
        """

    if get_jwt_identity() != "ADMIN":
        return jsonify({'error': 'Forbidden'}), 403

    data = request.get_json()
    validate_character_edit_data(data)
    return jsonify(edit_character(data)), 200


@selection_route.route('/delete_character', methods=['DELETE'])
@jwt_required()
def delete_character_route():
    """
        DELETE /delete_character

        Soft‑delete a character by ID.

        Body JSON:
            { "character_id": int }

        Returns:
            JSON of the deleted character, 200
        """

    data = request.get_json()
    validate_character_delete_data(data)
    char = set_delete_character(data, True)
    return jsonify(char.to_dict()), 200


@selection_route.route('/recover_character', methods=['PATCH'])
@jwt_required()
def recover_character_route():
    """
        PATCH /recover_character

        Recover a previously soft‑deleted character.

        Body JSON:
            { "character_id": int }

        Returns:
            JSON of the recovered character, 200
        """
    data = request.get_json()
    validate_character_delete_data(data)
    char = set_delete_character(data, False)
    return jsonify(char.to_dict()), 200

