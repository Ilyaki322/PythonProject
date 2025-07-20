from db import db

"""
character module

Defines the Character SQLAlchemy model for storing players,
their appearance/equipment, and associated inventory records.
"""


class Character(db.Model):
    """
        Represents a player’s avatar and stats in the 'characters' table.

        Attributes:
            is_deleted (bool): Soft‑delete flag, defaults to False.
            id (int): Primary key.
            name (str): Character name, required.
            level (int): Character level, default 1.
            money (int): In‑game currency, default 0.
            hair, helmet, beard, armor, pants, weapon, back (int):
                IDs of equipped items, each defaults to 0.
            user_id (int): Foreign key to users.id.
            user (User): Back‑reference to owner.
            inventory (list[CharacterInventory]):
                All inventory entries for this character.
        """

    __tablename__ = 'characters'

    is_deleted = db.Column(db.Boolean, default=False, nullable=False)

    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(80), nullable=False)
    level = db.Column(db.Integer, default=1)
    money = db.Column(db.Integer, default=0)

    hair = db.Column(db.Integer, default=0)
    helmet = db.Column(db.Integer, default=0)
    beard = db.Column(db.Integer, default=0)
    armor = db.Column(db.Integer, default=0)
    pants = db.Column(db.Integer, default=0)
    weapon = db.Column(db.Integer, default=0)
    back = db.Column(db.Integer, default=0)

    user_id = db.Column(db.Integer, db.ForeignKey('users.id'), nullable=False)
    user = db.relationship('User', back_populates='characters')

    inventory = db.relationship("CharacterInventory", back_populates="character", cascade="all, delete-orphan")

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
            'user_id': self.user_id,
            'money': self.money,
        }

    def get_inventory_dict(self):
        return [
            {
                'item': entry.item.to_dict(),
                'count': entry.count,
                'index': entry.index
            }
            for entry in self.inventory
        ]
