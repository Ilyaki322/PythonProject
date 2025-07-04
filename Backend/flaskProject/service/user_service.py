import re
import os

from flask import jsonify

from db import db, bcrypt
from google.oauth2 import id_token
from google.auth.transport import requests as google_requests
from models.user import User


def authenticate_or_create_google_user(id_token_str: str):
    try:
        idinfo = id_token.verify_oauth2_token(
            id_token_str,
            google_requests.Request(),
            os.getenv("GOOGLE_CLIENT_ID")
        )
    except ValueError:
        return {"success": False, "message": "Invalid Google token"}

    email = idinfo.get("email")
    if not email:
        return {"success": False, "message": "Google token missing email"}

    user = User.query.filter_by(email=email).first()
    if not user:
        # auto-generate a password; they’ll log in only via Google
        random_pw = bcrypt.generate_password_hash(os.urandom(16)).decode()
        user = User(
            username=email.split("@")[0],
            email=email,
            password=random_pw
        )
        db.session.add(user)
        db.session.commit()

    return {"success": True, "user_id": user.id}


def authenticate_user(username: str, password: str):
    if not username or not password:
        return {'status': 'error', 'message': 'Please enter your username and password'}

    user = User.query.filter_by(username=username).first()
    if user and bcrypt.check_password_hash(user.password, password):
        return {'status': 'success', 'message': 'Login successful', 'user_id': user.id}

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


def get_users():
    users = User.query.all()
    return jsonify({
        "users": [u.to_dict() for u in users]
    })
