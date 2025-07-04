import requests
from urllib.parse import urlencode
from flask import (Blueprint, current_app, redirect, request, abort, jsonify)
from flask_jwt_extended import create_access_token
from service.user_service import authenticate_or_create_google_user
from uuid import uuid4

oauth_bp = Blueprint("oauth_bp", __name__, url_prefix="/login")
token_store = {}


@oauth_bp.route("/google")
def google_authorize():
    client_state = request.args.get("state")
    state = client_state if client_state else str(uuid4())
    token_store[state] = None
    # generate or accept a client-provided state

    params = {
        "client_id":     current_app.config["GOOGLE_CLIENT_ID"],
        "response_type": "code",
        "scope":         "openid email profile",
        "redirect_uri":  current_app.config["GOOGLE_REDIRECT_URI"],  # http://localhost:5000/login/google/callback
        "state":         state,
        "access_type":   "offline",
        "prompt":        "consent",
    }

    return redirect("https://accounts.google.com/o/oauth2/v2/auth?" + urlencode(params))


@oauth_bp.route("/google/callback")
def google_callback():
    code = request.args.get("code")
    state = request.args.get("state")
    if not code or state not in token_store:
        abort(400, "Missing code or invalid state")

    # 2) Exchange code for ID token
    resp = requests.post("https://oauth2.googleapis.com/token", data={
            "code":          code,
            "client_id":     current_app.config["GOOGLE_CLIENT_ID"],
            "client_secret": current_app.config["GOOGLE_CLIENT_SECRET"],
            "redirect_uri":  current_app.config["GOOGLE_REDIRECT_URI"],
            "grant_type":    "authorization_code"},
    )

    resp.raise_for_status()
    id_token_str = resp.json().get("id_token")

    # 3) Verify & issue our JWT
    result = authenticate_or_create_google_user(id_token_str)
    jwt = create_access_token(identity=str(result["user_id"]))

    # 4) store it under that state
    token_store[state] = jwt

    return """
      <html><body>
        <h2>Login successful!</h2>
        <p>Return to the game; your session is ready.</p>
      </body></html>
    """


@oauth_bp.route("/status")
def login_status():
    state = request.args.get("state")
    token = token_store.get(state)
    if token:
        # (optional) delete token_store[state] here
        return jsonify(token=token), 200
    return "", 204
