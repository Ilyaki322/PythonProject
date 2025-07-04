from flask import jsonify
from models.item import Item
from models.inventory import CharacterInventory
from db import db


def update_items(items_json):
    print(len(items_json))
    for item_data in items_json:
        guid = item_data.get('id')

        if not guid or db.session.query(Item).filter_by(id=guid).first():
            continue

        new_item = Item(
            id=guid,
            name=item_data.get('name'),
            description=item_data.get('description'),
            icon=bytes.fromhex(item_data.get('icon')),
        )

        db.session.add(new_item)

    db.session.commit()


def get_char_inventory(char_id):
    inventory = CharacterInventory.query.filter_by(character_id=char_id).all()

    return jsonify([
        {
            'item': entry.item.to_dict() if entry.item else None,
            'count': entry.count,
            'index': entry.index
        }
        for entry in inventory
    ])
