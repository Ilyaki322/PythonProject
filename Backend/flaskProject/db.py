from flask_bcrypt import Bcrypt
from flask_sqlalchemy import SQLAlchemy

"""
Initializes Flask extensions for use throughout the application:
- db: SQLAlchemy ORM integration
- bcrypt: Password hashing utilities via Flask‑Bcrypt
"""

db = SQLAlchemy()
bcrypt = Bcrypt()
