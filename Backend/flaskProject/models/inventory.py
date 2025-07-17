from db import db


class CharacterInventory(db.Model):
    __tablename__ = 'character_inventory'

    character_id = db.Column(db.Integer, db.ForeignKey('characters.id'), primary_key=True)
    index = db.Column(db.Integer, nullable=False, default=0, primary_key=True)

    item_id = db.Column(db.String(36), db.ForeignKey('items.id'), nullable=False)

    count = db.Column(db.Integer, nullable=False, default=1)

    character = db.relationship("Character", back_populates="inventory")
    item = db.relationship("Item")
