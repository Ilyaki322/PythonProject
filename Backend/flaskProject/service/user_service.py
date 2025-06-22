from models.user import User
import re
from db import db
from db import bcrypt


def authenticate_user(username: str, password: str):
    if not username or not password:
        return {'status': 'error', 'message': 'Please enter your username and password'}

    user = User.query.filter_by(username=username).first()
    if user and bcrypt.check_password_hash(user.password, password):
        return {'status': 'success', 'message': 'Login successful'}

    return {'status': 'error', 'message': 'Wrong username or password'}


def register_user(username: str, password: str, email: str):
    if not re.fullmatch(r'[a-zA-Z]{4,10}', username):
        return {'success': False, 'field': 'username', 'error': 'Username must be 4-10 letters only'}

    if not re.fullmatch(r'[a-zA-Z0-9]{5,}', password):
        return {'success': False, 'field': 'password', 'error': 'Password must be at least 5 characters and contain '
                                                                'only letters and digits'}

    if not re.fullmatch(r"[^@]+@[^@]+\.[^@]+", email):
        return {'success': False, 'field': 'email', 'error': 'Invalid email format'}

    if User.query.filter_by(username=username).first():
        return {'success': False, 'field': 'username', 'error': 'Username already exists'}

    if User.query.filter_by(email=email).first():
        return {'success': False, 'field': 'email', 'error': 'Email already registered'}

    hashed_password = bcrypt.generate_password_hash(password).decode('utf-8')
    new_user = User(username=username, email=email, password=hashed_password)
    db.session.add(new_user)
    db.session.commit()

    return {'success': True, 'message': 'User registered successfully'}
