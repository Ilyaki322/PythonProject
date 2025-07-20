from db import db
from datetime import datetime

"""
match module

Defines the Match SQLAlchemy model for recording the start time,
participants, and outcome of completed matches in the 'matches' table.
"""


class Match(db.Model):
    """
        Represents an entry in the 'matches' table recording a completed game.

        Attributes:
            id (int): Primary key, unique match identifier.
            match_start (datetime): UTC timestamp of when the match began.
            player_1_id (int): Foreign key to users.id for the first participant.
            player_2_id (int): Foreign key to users.id for the second participant.
            winner_id (int | None): Foreign key to users.id for the winner, or None if draw.
            player_1 (User): Relationship to the first User object.
            player_2 (User): Relationship to the second User object.
            winner (User | None): Relationship to the winning User, if any.
        """

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
