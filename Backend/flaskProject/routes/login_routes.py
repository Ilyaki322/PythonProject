from flask import Blueprint, request
from service.user_service import authenticate_user, register_user, get_users, set_deleted
from service.stats_service import *
from flask_jwt_extended import create_access_token, jwt_required
from errors.validators import validate_user_delete

"""
login_route module

Handles user authentication, registration, retrieval, deletion, and data export
via Flask JWT tokens under the '/login' Blueprint.
"""

login_route = Blueprint('login_route', __name__)

ADMIN_USER = 'admin'
ADMIN_PASSWORD = 'admin'


@login_route.route('/login', methods=['POST'])
def login():
    """
        POST /login

        Authenticate a user with username and password.

        Body JSON:
            {
                "username": str,
                "password": str
            }

        Returns:
            JSON with 'message' and 'token' on success, HTTP 200;
            JSON with 'message' on failure, HTTP 400.
        """
    data = request.get_json()
    result = authenticate_user(data.get('username'), data.get('password'))

    if result['status'] == 'success':
        access_token = create_access_token(identity=str(result['user_id']))
        return jsonify({'message': result['message'], 'token': access_token}), 200
    else:
        return jsonify({'message': result['message']}), 400


@login_route.route('/login_admin', methods=['POST'])
def login_admin():
    """
        POST /login_admin

        Authenticate a hard‑coded admin user.

        Body JSON:
            {
                "username": str,
                "password": str
            }

        Returns:
            JSON with 'message' and 'token' on success, HTTP 200;
            JSON with 'message' on failure, HTTP 400.
        """
    data = request.get_json()
    username = data.get("username")
    password = data.get("password")

    if username == ADMIN_USER and password == ADMIN_PASSWORD:
        access_token = create_access_token(identity='ADMIN')
        return jsonify({'message': 'success', 'token': access_token}), 200

    return jsonify({'message': 'Username or password are incorrect'}), 400


@login_route.route('/register', methods=['POST'])
def register():
    """
        POST /register

        Register a new user account.

        Body JSON:
            {
                "username": str,
                "password": str,
                "email": str
            }

        Returns:
            JSON {'success': True, 'message': ...}, HTTP 200 on success;
            JSON {'success': False, 'field': str, 'error': str}, HTTP 400 on failure.
        """
    data = request.get_json()
    result = register_user(data.get('username'), data.get('password'), data.get('email'))

    if result.get('success'):
        return jsonify({'success': True, 'message': 'Registered successfully'}), 200
    else:
        return jsonify({'success': False, 'field': result['field'], 'error': result['error']}), 400


@login_route.route('/users', methods=['GET'])
@jwt_required()
def get_users_route():
    """
        GET /users

        List all non‑deleted users.

        Authentication:
            Requires valid JWT.

        Returns:
            JSON list of users, HTTP 200.
        """
    return get_users(False)


@login_route.route('/deleted_users', methods=['GET'])
@jwt_required()
def get_deleted_users_route():
    """
        GET /deleted_users

        List all soft‑deleted users.

        Authentication:
            Requires valid JWT.

        Returns:
            JSON {'users': [...]}, HTTP 200.
        """
    return jsonify({"users": get_users(True)}), 200


@login_route.route('/delete', methods=['PATCH'])
@jwt_required()
def delete_user_route():
    """
        PATCH /delete

        Soft‑delete a user account.

        Body JSON:
            { "user_id": int }

        Authentication:
            Requires valid JWT.

        Returns:
            JSON response from service, HTTP 200.
        """
    data = request.get_json()
    validate_user_delete(data)

    return set_deleted(data, True), 200


@login_route.route('/recover', methods=['PATCH'])
@jwt_required()
def recover_user_route():
    """
        PATCH /recover

        Recover a previously soft‑deleted user.

        Body JSON:
            { "user_id": int }

        Authentication:
            Requires valid JWT.

        Returns:
            JSON response from service, HTTP 200.
        """
    data = request.get_json()
    validate_user_delete(data)
    return set_deleted(data, False), 200


@login_route.route('/user_data/<user_id>', methods=['GET'])
@jwt_required()
def get_user_data_route(user_id):
    """
        GET /user_data/<user_id>

        Export a user’s match history to an Excel file.

        Path Params:
            user_id (str): ID of the user.

        Returns:
            A streaming Excel file attachment response, HTTP 200.
        """
    return export_user_matches_to_excel(user_id)
