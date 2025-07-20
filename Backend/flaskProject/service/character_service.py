from werkzeug.exceptions import NotFound

from models.character import Character
from models.user import User
from db import db

"""
service.character_service module

Provides business logic for character retrieval, creation, update, deletion,
and related operations, leveraging SQLAlchemy for database interactions.
"""


def get_characters(user_id, deleted):
    """
        Retrieve characters belonging to a user with optional soft-delete filter.

        Args:
            user_id: ID of the user whose characters to fetch.
            deleted: Whether to include deleted characters.

        Returns:
            List of character dictionaries.
        """
    characters = Character.query.filter(
        Character.user_id == user_id,
        Character.is_deleted == deleted
    ).all()

    return [c.to_dict() for c in characters]


def get_characters_deleted():
    """
        List all soft-deleted characters with their owner usernames.

        Returns:
            List of character dictionaries including a 'user_name' field.
        """
    characters = Character.query.filter(Character.is_deleted == True).all()

    character_data = []
    for c in characters:
        user = User.query.filter_by(id=c.user_id).first()
        character_dict = c.to_dict()
        character_dict['user_name'] = user.username if user else 'Unknown'
        character_data.append(character_dict)

    return character_data


def get_char_by_id(character_id):
    """
        Fetch a single character by its ID.

        Args:
            character_id: The primary key of the character.

        Returns:
            Dictionary representation of the character.

        Note:
            Calling to_dict on None will raise an AttributeError if not found.
        """
    character = Character.query.filter_by(id=character_id).first()
    return character.to_dict()


def add_character(data, user_id):
    """
        Create and persist a new character for a user.

        Args:
            data: Fields for the new character (name, level, gear IDs).
            user_id: The owner user's ID.

        Returns:
            Dictionary of the newly created character.
        """
    character = Character(
        name=data.get('name'),
        level=data.get('level', 1),
        hair=data.get('hair', 0),
        helmet=data.get('helmet', 0),
        beard=data.get('beard', 0),
        armor=data.get('armor', 0),
        pants=data.get('pants', 0),
        weapon=data.get('weapon', 0),
        back=data.get('back', 0),
        user_id=user_id
    )

    db.session.add(character)
    db.session.commit()

    return character.to_dict()


def edit_character(data):
    """
        Update an existing character's name and level.

        Args:
            data: Contains 'character_id', 'name', and 'level'.

        Returns:
            Dictionary of the updated character.
        """
    character = Character.query.get_or_404(data['character_id'])

    character.name = data['name']
    character.level = data['level']

    db.session.commit()

    return character.to_dict()


def set_delete_character(data, to_delete):
    """
        Soft-delete or recover a character.

        Args:
            data: Contains 'character_id'.
            to_delete: True to delete, False to recover.

        Returns:
            The Character instance after update.
        """
    character = Character.query.get_or_404(data['character_id'])

    character.is_deleted = to_delete
    db.session.commit()

    return character


def character_levelUp(character_id: int, new_level: int, current_gold: int):
    """
        Level up a character and adjust its gold.

        Args:
            character_id: The ID of the character to update.
            new_level: The new level to assign.
            current_gold: The updated gold total.

        Returns:
            Dictionary of the leveled-up character.

        """
    character = Character.query.get_or_404(character_id)
    if not character:
        raise NotFound(f"Character {character_id} not found")
    character.level = new_level
    character.money = current_gold
    db.session.commit()
    return character.to_dict()


def update_character_gold(character_id: int, amount: int):
    """
        Modify a character's gold by a specified amount.

        Args:
            character_id: The ID of the character to update.
            amount: Amount to add (or subtract if negative).

        Returns:
            Dictionary of the updated character.
        """
    character = Character.query.get_or_404(character_id)
    if not character:
        raise NotFound(f"Character {character_id} not found")
    character.money += amount
    db.session.commit()
    return character.to_dict()

