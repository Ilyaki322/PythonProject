# flask routes/oauth_routes.py

from uuid import uuid4
from flask import Blueprint, current_app, redirect, request, abort, jsonify
from flask_jwt_extended import create_access_token
from service.oauth_service import OAuthService
from service.user_service import authenticate_or_create_google_user
import requests
from urllib.parse import urlencode


"""
oauth_routes module

Defines OAuth 2.0 login flows for Google under the '/login' Blueprint,
handling authorization redirection, callback processing, and login status checks.
"""

oauth_bp = Blueprint("oauth_bp", __name__, url_prefix="/login")

@oauth_bp.route("/google")
def google_authorize():
    """
        GET /login/google

        Initiates Google OAuth 2.0 authorization by generating a CSRF‑safe state
        and redirecting the user to Google's consent page.

        - Generates a random state nonce stored via OAuthService. :contentReference[oaicite:0]{index=0}
        - Constructs the authorization URL with client ID, scope, redirect URI,
          and state parameter.
        - Returns a Flask redirect to the Google OAuth endpoint.
        """
    state = OAuthService.create_state(request.args.get("state"))

    params = {
        "client_id":     current_app.config["GOOGLE_CLIENT_ID"],
        "response_type": "code",
        "scope":         "openid email profile",
        "redirect_uri":  current_app.config["GOOGLE_REDIRECT_URI"],
        "state":         state,
        "access_type":   "offline",
        "prompt":        "consent",
    }
    return redirect("https://accounts.google.com/o/oauth2/v2/auth?" + urlencode(params))

@oauth_bp.route("/google/callback")
def google_callback():
    """
        GET /login/google/callback

        Handles the OAuth callback from Google:

        - Validates presence of `code` and `state`; aborts with 400 on failure.
        - Exchanges authorization code for tokens via Google's token endpoint.
        - Parses the ID token and creates or retrieves a user.
        - Issues a JWT access token and associates it with the state entry.
        - Returns an HTML confirmation page instructing the client to proceed.
        """
    code = request.args.get("code")
    state = request.args.get("state")
    entry = OAuthService.get_entry(state)

    if not code or entry is None:
        abort(400, "Missing code or invalid state")

    # exchange code
    resp = requests.post(
        "https://oauth2.googleapis.com/token",
        data={
            "code":          code,
            "client_id":     current_app.config["GOOGLE_CLIENT_ID"],
            "client_secret": current_app.config["GOOGLE_CLIENT_SECRET"],
            "redirect_uri":  current_app.config["GOOGLE_REDIRECT_URI"],
            "grant_type":    "authorization_code",
        },
    )
    resp.raise_for_status()
    id_token_str = resp.json().get("id_token")

    result = authenticate_or_create_google_user(id_token_str)
    print(result)

    jwt = create_access_token(identity=str(result["user_id"]))
    OAuthService.set_token(state, jwt)

    return """
      <html><body>
        <h2>Login successful!</h2>
        <p>Return to the game; your session is ready.</p>
      </body></html>
    """

@oauth_bp.route("/status")
def login_status():
    """
        GET /login/status

        Checks if a JWT has been issued for the given state:

        - Reads `state` from query parameters.
        - Retrieves and clears the associated token via OAuthService.
        - Returns JSON `{ "token": <jwt> }` with HTTP200 if available.
        - Returns empty body with HTTP204 if no token yet.
        """
    state = request.args.get("state")
    token = OAuthService.pop_token(state)

    if token:
        return jsonify(token=token), 200
    return "", 204