from db import db


class User(db.Model):
    __tablename__ = 'users'  # Avoid reserved word "user"

    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(50), unique=True, nullable=False)
    password = db.Column(db.String(100), nullable=False)
    email = db.Column(db.String(100), unique=True, nullable=False)

    characters = db.relationship('Character', back_populates='user', cascade='all, delete-orphan')
