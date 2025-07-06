from flask import jsonify
from models.character import Character
from db import db


def get_characters(user_id, deleted):
    characters = Character.query.filter(
        Character.user_id == user_id,
        Character.is_deleted == deleted
    ).all()
    return jsonify({
        "count": len(characters),
        "characters": [c.to_dict() for c in characters]
    })


def get_characters_deleted():
    characters = Character.query.filter(
        Character.is_deleted == True
    ).all()

    return jsonify({
        "count": len(characters),
        "characters": [c.to_dict() for c in characters]
    })


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

    return jsonify(character.to_dict()), 201


def edit_character(data):
    character = Character.query.filter_by(id=data['character_id']).first()

    if not character:
        return jsonify({"success": False}), 404

    if 'name' in data:
        character.name = data['name']
    if 'level' in data:
        character.level = data['level']

    db.session.commit()

    return jsonify({"success": True, "character": character.to_dict()}), 200


def set_delete_character(data, to_delete):
    character = Character.query.filter_by(id=data['character_id']).first()

    if not character:
        return jsonify({"success": False}), 404

    character.is_deleted = to_delete
    db.session.commit()

    return jsonify({"success": True}), 200


