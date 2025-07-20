from db import db

"""
character_inventory module

Defines the CharacterInventory model, which represents an item slot
in a character’s inventory and manages counts and relationships.
"""


class CharacterInventory(db.Model):
    """
        Maps the 'character_inventory' table to Python.

        Attributes:
            character_id (int): Foreign key to characters.id; part of composite PK.
            index (int): Slot position within the inventory; part of composite PK.
            item_id (str): Foreign key to items.id.
            count (int): Quantity of the given item in this slot.
            character (Character): Back‑populated relationship to Character.
            item (Item): Relationship to the Item model.
        """

    __tablename__ = 'character_inventory'

    character_id = db.Column(db.Integer, db.ForeignKey('characters.id'), primary_key=True)
    index = db.Column(db.Integer, nullable=False, default=0, primary_key=True)

    item_id = db.Column(db.String(36), db.ForeignKey('items.id'), nullable=False)

    count = db.Column(db.Integer, nullable=False, default=1)

    character = db.relationship("Character", back_populates="inventory")
    item = db.relationship("Item")
