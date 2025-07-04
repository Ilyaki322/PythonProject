from flask import Blueprint, request, jsonify
from service.user_service import authenticate_user, register_user, get_users
from flask_jwt_extended import create_access_token

login_route = Blueprint('login_route', __name__)


@login_route.route('/login', methods=['POST'])
def login():
    data = request.get_json()
    result = authenticate_user(data.get('username'), data.get('password'))

    if result['status'] == 'success':
        access_token = create_access_token(identity=str(result['user_id']))
        return jsonify({'message': result['message'], 'token': access_token}), 200
    else:
        return jsonify({'message': result['message']}), 400


@login_route.route('/register', methods=['POST'])
def register():
    data = request.get_json()
    result = register_user(data.get('username'), data.get('password'), data.get('email'))

    if result.get('success'):
        return jsonify({'success': True, 'message': 'Registered successfully'}), 200
    else:
        return jsonify({'success': False, 'field': result['field'], 'error': result['error']}), 400


@login_route.route('/users', methods=['GET'])
def get_users_route():
    return get_users()
