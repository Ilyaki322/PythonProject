from uuid import uuid4
from db import db
from models.OAuthState import OAuthState

"""
oauth_service module

Encapsulates OAuth 2.0 state management, providing CRUD operations on
the OAuthState model to safely generate, retrieve, and consume CSRF‑safe
state nonces alongside their associated JWTs.
"""


class OAuthService:
    """
        Service layer for handling OAuth2.0 state tokens.

        This class implements a stateless interface for:
          - Creating new state nonces (with optional reuse of a client‑supplied state)
          - Retrieving stored state entries
          - Associating JWTs with state upon successful callback
          - Consuming (popping) JWTs in a one‑time read and delete operation

        All methods are defined as @staticmethod to allow direct invocation
        without instantiating the class.
        """
    @staticmethod
    def create_state(req_state=None, expiration_seconds=300):
        """
        Generate a new OAuth state entry in the database with an optional expiration.
        Returns the new state string.
        """
        state = req_state or str(uuid4())
        db.session.add(OAuthState(state=state, token=None))
        db.session.commit()
        return state

    @staticmethod
    def get_entry(state):
        """
        Retrieve the OAuthState entry by state. Returns None if not found.
        """
        return OAuthState.query.get(state)

    @staticmethod
    def set_token(state, jwt_token):
        """
        Associate a JWT token with the given state.
        """
        entry = OAuthService.get_entry(state)
        if entry:
            entry.token = jwt_token
            db.session.commit()
        return entry

    @staticmethod
    def pop_token(state):
        """
        Retrieve and delete the OAuthState entry, returning the token if present.
        """
        entry = OAuthService.get_entry(state)
        token = None
        if entry and entry.token:
            token = entry.token
            db.session.delete(entry)
            db.session.commit()
        return token
