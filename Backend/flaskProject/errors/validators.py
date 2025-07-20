from werkzeug.exceptions import BadRequest


def validate_required_fields(data, required_fields):
    """
        Ensure `data` contains all fields listed in `required_fields`.
        Raises BadRequest if the body is missing or if any field is absent.
        """
    if not data:
        raise BadRequest("Missing JSON body")

    # Identify any fields the client failed to provide
    missing = [field for field in required_fields if field not in data]
    if missing:
        print(missing)
        raise BadRequest(f"Missing fields: {', '.join(missing)}")


def validate_character_data(data):
    """
        Validate payload when creating a new character.
        Expects all appearance and equipment fields plus name and level.
        """
    validate_required_fields(data, ['name', 'level', 'hair', 'helmet', 'beard', 'armor', 'pants', 'weapon', 'back'])


def validate_character_edit_data(data):
    """
        Validate payload when editing an existing character.
        Requires at minimum the character’s ID, its updated name, and level.
        """
    validate_required_fields(data, ['name', 'character_id', 'level'])


def validate_character_delete_data(data):
    """
        Validate payload for deleting a character.
        Only the character’s ID is needed.
        """
    validate_required_fields(data, ['character_id'])


def validate_slot_update(data):
    """
        Validate payload for updating an inventory slot.
        Needs character ID, slot index, item ID, and the new count.
        """
    validate_required_fields(data, ['character_id', 'index', 'item_id', 'count'])


def validate_user_delete(data):
    """
        Validate payload for deleting a user.
        Requires the user’s ID.
        """
    validate_required_fields(data, ['user_id'])

