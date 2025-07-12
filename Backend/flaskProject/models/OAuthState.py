from db import db


class OAuthState(db.Model):
    state = db.Column(db.String, primary_key=True)
    token = db.Column(db.String, nullable=True)
