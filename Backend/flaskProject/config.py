import os

"""
Configuration for development environment.

Loads settings from environment variables with fallback defaults,
including JWT secret, database URI, and Google OAuth2 credentials.
"""


class DevConfig:
    """
        Development configuration for the Flask application.

        Attributes:
            JWT_SECRET_KEY (str): Secret key for signing JWTs.
            SQLALCHEMY_DATABASE_URI (str): Database connection URI.
            SQLALCHEMY_TRACK_MODIFICATIONS (bool): Disable SQLAlchemy event tracking.
            GOOGLE_CLIENT_ID (str): OAuth2 client ID for Google login.
            GOOGLE_CLIENT_SECRET (str): OAuth2 client secret for Google login.
            GOOGLE_REDIRECT_URI (str): OAuth2 callback URL.
            FRONTEND_URI (str): Custom URI scheme for returning tokens to front end.
        """

    # JWT
    JWT_SECRET_KEY = os.getenv('SECRET_KEY', "wellwellwell322")

    # Flask db
    SQLALCHEMY_DATABASE_URI = "postgresql://admin:Aa123456@localhost:5433/pyproj"
    SQLALCHEMY_TRACK_MODIFICATIONS = False

    GOOGLE_CLIENT_ID = os.getenv("GOOGLE_CLIENT_ID", '480759518763-vig8uc0379tat0cg0a90orccqut7ioup.apps'
                                                     '.googleusercontent.com')

    GOOGLE_CLIENT_SECRET = os.getenv("GOOGLE_CLIENT_SECRET", "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")

    GOOGLE_REDIRECT_URI = os.getenv("GOOGLE_REDIRECT_URI", "http://localhost:5000/login/google/callback")
    FRONTEND_URI = os.getenv("FRONTEND_URI", "mygame://oauth2?token=")

