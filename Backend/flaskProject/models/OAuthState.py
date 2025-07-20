from db import db

"""
oauth_state module

Defines the OAuthState SQLAlchemy model for temporarily storing
OAuth 2.0 state tokens and their associated response tokens.
"""


class OAuthState(db.Model):
    """
        Represents a pending OAuth 2.0 flow state and its eventual token.

        Attributes:
            state (str): Primary key. A random string used to correlate an
                OAuth request and callback.
            token (str | None): The access or refresh token returned by
                the OAuth provider, populated after callback.
        """

    state = db.Column(db.String, primary_key=True)
    token = db.Column(db.String, nullable=True)
