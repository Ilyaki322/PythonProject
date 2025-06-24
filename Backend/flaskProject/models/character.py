from db import db


class Character(db.Model):
    def to_dict(self):
        return {
            'id': self.id,
            'name': self.name,
            'level': self.level,
            'hair': self.hair,
            'helmet': self.helmet,
            'beard': self.beard,
            'armor': self.armor,
            'pants': self.pants,
            'weapon': self.weapon,
            'back': self.back,
            'user_id': self.user_id
        }

    __tablename__ = 'characters'

    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(80), nullable=False)
    level = db.Column(db.Integer, default=1)

    hair = db.Column(db.Integer, default=0)
    helmet = db.Column(db.Integer, default=0)
    beard = db.Column(db.Integer, default=0)
    armor = db.Column(db.Integer, default=0)
    pants = db.Column(db.Integer, default=0)
    weapon = db.Column(db.Integer, default=0)
    back = db.Column(db.Integer, default=0)

    user_id = db.Column(db.Integer, db.ForeignKey('users.id'), nullable=False)
    user = db.relationship('User', back_populates='characters')
