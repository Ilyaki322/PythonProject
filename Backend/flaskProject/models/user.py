from db import db

"""
user module

Defines the User SQLAlchemy model for application accounts,
including authentication fields and related Character records.
"""


class User(db.Model):
    """
        Represents an application user in the 'users' table.

        Attributes:
            is_deleted (bool): Soft‑delete flag; defaults to False.
            id (int): Primary key for the user.
            username (str): Unique login name, up to 50 characters.
            password (str): Hashed password string, up to 100 characters.
            email (str): Unique email address, up to 100 characters.
            characters (list[Character]):
                Back‑populated relationship to the user's characters,
                cascades deletes to associated Character records.
        """

    __tablename__ = 'users'

    is_deleted = db.Column(db.Boolean, default=False, nullable=False)
    
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(50), unique=True, nullable=False)
    password = db.Column(db.String(100), nullable=False)
    email = db.Column(db.String(100), unique=True, nullable=False)

    characters = db.relationship('Character', back_populates='user', cascade='all, delete-orphan')

    def to_dict(self):
        return {
            'id': self.id,
            'username': self.username,
            'email': self.email,
        }
