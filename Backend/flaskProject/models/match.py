from db import db
from datetime import datetime


class Match(db.Model):
    __tablename__ = 'matches'

    id = db.Column(db.Integer, primary_key=True)
    match_start = db.Column(db.DateTime, default=datetime.utcnow, nullable=False)

    player_1_id = db.Column(db.Integer, db.ForeignKey('users.id'), nullable=False)
    player_2_id = db.Column(db.Integer, db.ForeignKey('users.id'), nullable=False)
    winner_id = db.Column(db.Integer, db.ForeignKey('users.id'), nullable=True)

    player_1 = db.relationship('User', foreign_keys=[player_1_id])
    player_2 = db.relationship('User', foreign_keys=[player_2_id])
    winner = db.relationship('User', foreign_keys=[winner_id])

    def to_dict(self):
        return {
            'id': self.id,
            'match_start': self.match_start.isoformat(),
            'player_1_id': self.player_1_id,
            'player_2_id': self.player_2_id,
            'winner_id': self.winner_id,
        }
