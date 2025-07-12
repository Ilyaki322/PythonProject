from flask import Blueprint, request, jsonify
from service.user_service import authenticate_user, register_user, get_users, set_deleted
from service.stats_service import *
from flask_jwt_extended import create_access_token, jwt_required
from errors.validators import validate_user_delete

login_route = Blueprint('login_route', __name__)

ADMIN_USER = 'admin'
ADMIN_PASSWORD = 'admin'


@login_route.route('/login', methods=['POST'])
def login():
    data = request.get_json()
    result = authenticate_user(data.get('username'), data.get('password'))

    if result['status'] == 'success':
        access_token = create_access_token(identity=str(result['user_id']))
        return jsonify({'message': result['message'], 'token': access_token}), 200
    else:
        return jsonify({'message': result['message']}), 400


@login_route.route('/login_admin', methods=['POST'])
def login_admin():
    data = request.get_json()
    username = data.get("username")
    password = data.get("password")

    if username == ADMIN_USER and password == ADMIN_PASSWORD:
        access_token = create_access_token(identity='ADMIN')
        return jsonify({'message': 'success', 'token': access_token}), 200

    return jsonify({'message': 'Username or password are incorrect'}), 400


@login_route.route('/register', methods=['POST'])
def register():
    data = request.get_json()
    result = register_user(data.get('username'), data.get('password'), data.get('email'))

    if result.get('success'):
        return jsonify({'success': True, 'message': 'Registered successfully'}), 200
    else:
        return jsonify({'success': False, 'field': result['field'], 'error': result['error']}), 400


@login_route.route('/users', methods=['GET'])
@jwt_required()
def get_users_route():
    return get_users(False)


@login_route.route('/deleted_users', methods=['GET'])
@jwt_required()
def get_deleted_users_route():
    return jsonify({"users": get_users(True)}), 200


@login_route.route('/delete', methods=['PATCH'])
@jwt_required()
def delete_user_route():
    data = request.get_json()
    validate_user_delete(data)

    return set_deleted(data, True), 200


@login_route.route('/recover', methods=['PATCH'])
@jwt_required()
def recover_user_route():
    data = request.get_json()
    validate_user_delete(data)

    return set_deleted(data, False), 200


@login_route.route('/user_data/<user_id>', methods=['GET'])
@jwt_required()
def get_user_data_route(user_id):
    return export_user_matches_to_excel(user_id)
