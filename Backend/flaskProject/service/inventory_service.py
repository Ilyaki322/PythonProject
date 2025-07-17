from models.item import Item
from models.inventory import CharacterInventory
from db import db
import base64


def update_items(items_json):
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
    print(inventory)
    return [
        {
            'item': entry.item.to_dict() if entry.item else None,
            'count': entry.count,
            'index': entry.index
        }
        for entry in inventory
    ]


def get_all_items():
    items = Item.query.all()

    result = []
    for item in items:
        icon_b64 = base64.b64encode(item.icon).decode('utf-8') if item.icon else None
        result.append({
            "id": item.id,
            "name": item.name,
            "description": item.description,
            "icon": icon_b64,
        })

    return result


def update_slot(data):
    entry = CharacterInventory.query.filter_by(character_id=data['character_id'], index=data['index']).first()
    code = 200

    if entry:
        if data['item_id'] is None:
            db.session.delete(entry)
        else:
            entry.item_id = data['item_id']
            entry.count = data['count']

    else:
        if data['item_id'] is not None:
            new_entry = CharacterInventory(
                character_id=data['character_id'],
                item_id=data['item_id'],
                count=data['count'],
                index=data['index']
            )
            db.session.add(new_entry)
            code = 201

    db.session.commit()
    return {"success": True}, code


def buy_item(data):
    entry = CharacterInventory.query.filter_by(character_id=data['character_id'], index=data['index']).first()

    if not entry:
        new_entry = CharacterInventory(
            character_id=data['character_id'],
            item_id=data['item_id'],
            count=data['count'],
            index=data['index']
        )
        db.session.add(new_entry)
    else:
        if data['item_id'] == entry.item_id:
            entry.count += data['count']
        else:
            return {"success": False}

    db.session.commit()
    return {"success": True}


def sell_item(data):
    entry = CharacterInventory.query.filter_by(
        character_id=data['character_id'],
        item_id=data['item_id'],
        count=data['count'],
        index=data['index']).first()

    if not entry:
        return {"success": False}
    else:
        entry.count -= data['count']
        if entry.count <= 0:
            db.session.delete(entry)

        return {"success": True}


def swap_items(data):
    entry_src = CharacterInventory.query.filter_by(character_id=data['character_id'], index=data['indexSrc']).first()
    entry_dst = CharacterInventory.query.filter_by(character_id=data['character_id'], index=data['indexDest']).first()

    if not entry_src:
        return {"success": False, "error": "Source slot not found"}, 404

    if entry_dst is None or entry_dst.item_id is None:
        if entry_dst is None:
            dst = CharacterInventory(
                character_id=data['character_id'],
                item_id=entry_src.item_id,
                count=entry_src.count,
                index=data['indexDest']
            )
            db.session.add(dst)
        else:
            entry_dst.item_id = entry_src.item_id
            entry_dst.count = entry_src.count
        db.session.delete(entry_src)

    else:
        # Both occupied: swap item_id and count
        entry_dst.item_id, entry_src.item_id = entry_src.item_id, entry_dst.item_id
        entry_dst.count, entry_src.count = entry_src.count, entry_dst.count

    db.session.commit()
    return {"success": True}



