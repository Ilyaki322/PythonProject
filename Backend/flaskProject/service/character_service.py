from werkzeug.exceptions import NotFound

from models.character import Character
from models.user import User
from db import db


def get_characters(user_id, deleted):
    characters = Character.query.filter(
        Character.user_id == user_id,
        Character.is_deleted == deleted
    ).all()

    return [c.to_dict() for c in characters]


def get_characters_deleted():
    characters = Character.query.filter(Character.is_deleted == True).all()

    character_data = []
    for c in characters:
        user = User.query.filter_by(id=c.user_id).first()
        character_dict = c.to_dict()
        character_dict['user_name'] = user.username if user else 'Unknown'
        character_data.append(character_dict)

    return character_data


def get_char_by_id(character_id):
    character = Character.query.filter_by(id=character_id).first()
    return character.to_dict()


def add_character(data, user_id):
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
    character = Character.query.get_or_404(data['character_id'])

    character.name = data['name']
    character.level = data['level']

    db.session.commit()

    return character.to_dict()


def set_delete_character(data, to_delete):
    character = Character.query.get_or_404(data['character_id'])

    character.is_deleted = to_delete
    db.session.commit()

    return character


def character_levelUp(character_id: int, new_level: int, current_gold: int):
    character = Character.query.get_or_404(character_id)
    if not character:
        raise NotFound(f"Character {character_id} not found")
    character.level = new_level
    character.money = current_gold
    db.session.commit()
    return character.to_dict()


def update_character_gold(character_id: int, amount: int):
    character = Character.query.get_or_404(character_id)
    if not character:
        raise NotFound(f"Character {character_id} not found")
    character.money += amount
    db.session.commit()
    return character.to_dict()

