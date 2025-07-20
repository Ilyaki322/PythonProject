from db import db
import uuid
import base64

"""
item module

Defines the Item SQLAlchemy model for storing information about
in‑game items, including an auto‑generated UUID, display name,
description, and optional icon binary data.
"""


class Item(db.Model):
    """
        Represents a collectible or equippable item in the 'items' table.

        Attributes:
            id (str): Primary key, generated as a UUID4 string by default.
            name (str): Human‑readable name, up to 100 characters.
            description (str | None): Detailed text about the item.
            icon (bytes | None): Raw binary for an icon image (e.g., PNG data).

        Relationships:
            None directly—other tables may reference Item.id as a foreign key.
        """


    __tablename__ = 'items'

    id = db.Column(db.String(36), primary_key=True, default=lambda: str(uuid.uuid4()))
    name = db.Column(db.String(100), nullable=False)
    description = db.Column(db.Text, nullable=True)
    icon = db.Column(db.LargeBinary, nullable=True)

    def to_dict(self):
        return {
            'id': self.id,
            'name': self.name,
            'description': self.description,
        }
