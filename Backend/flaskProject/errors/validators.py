from werkzeug.exceptions import BadRequest


def validate_required_fields(data, required_fields):
    if not data:
        raise BadRequest("Missing JSON body")

    missing = [field for field in required_fields if field not in data]
    if missing:
        print(missing)
        raise BadRequest(f"Missing fields: {', '.join(missing)}")


def validate_character_data(data):
    validate_required_fields(data, ['name', 'level', 'hair', 'helmet', 'beard', 'armor', 'pants', 'weapon', 'back'])


def validate_character_edit_data(data):
    validate_required_fields(data, ['name', 'character_id', 'level'])


def validate_character_delete_data(data):
    validate_required_fields(data, ['character_id'])


def validate_slot_update(data):
    validate_required_fields(data, ['character_id', 'index', 'item_id', 'count'])

