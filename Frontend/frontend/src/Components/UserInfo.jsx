import IconButton from "./UtilityComponents/IconButton";
import Card from 'react-bootstrap/Card';
import useAPI from "../CustomHooks/useAPI";
import ItemsAccordion from "./ItemsAccordion";
import AlertComponent from "./UtilityComponents/AlertComponent";
import { Spinner } from "react-bootstrap";
import { useState } from "react";

/**
 * A page to see all users characters and their inventory.
 * Fetches users characters and all items from db.
 * @param {object} user - users data.
 * @param {function} onBack - returns to UserPage.
 * @returns react component.
 */
const UserInfo = ({ user, onBack }) => {
    const [charData, charLoading, charError, setCharData] = useAPI(`http://localhost:5000/characters/${user.id}`, 'failed to fetch users characters');
    const [itemData, itemLoading, itemError] = useAPI(`http://localhost:5000/inventory/items`, 'failed to fetch items');

    const [deleteError, setDeleteError] = useState(null);
    const [itemEditError, setItemEditError] = useState(null);

    const [editingId, setEditingId] = useState(null);
    const [editedName, setEditedName] = useState('');
    const [editedLevel, setEditedLevel] = useState(0);

    /**
     * renders a single bootstrap card entry for each character.
     * @param {object} character - single character object as gotten from the db.
     * @returns html/jsx piece
     */
    function renderCard(character, index) {
        if (character.id === editingId) return renderEditCard(character, index)
        return (
            <Card key={character.id} className="mt-2">
                <Card.Body>
                    <Card.Title className="text-center">Character #{index}</Card.Title>

                    <div className="d-flex gap-2">
                        <IconButton icon="bi bi-trash-fill" className='bg-danger' onClick={() => deleteChar(character.id)} />
                        <IconButton icon="bi bi-pencil-fill" className='bg-warning' onClick={() => {
                            setEditingId(character.id);
                            setEditedName(character.name);
                            setEditedLevel(character.level);
                        }} />
                    </div>

                    <Card.Text className="mt-2 mb-0"><strong>Name:</strong> {character.name}</Card.Text>
                    <Card.Text><strong>Level:</strong> {character.level}</Card.Text>

                    <ItemsAccordion charID={character.id} itemData={itemData} />
                </Card.Body>
            </Card>
        );
    };

    /**
     * A special render for when an edit button is clicked.
     * @param {object} character - single character object as gotten from the db.
     * @returns html/jsx piece
     */
    function renderEditCard(character, index) {
        return (
            <Card key={character.id} className="mt-2 bg-warning">
                <Card.Body>
                    <Card.Title className="text-center">Character #{index}</Card.Title>

                    <div className="d-flex gap-2">
                        <IconButton icon="bi bi-trash-fill" className='bg-danger' onClick={() => deleteChar(character.id)} />
                        <IconButton icon="bi bi-check-lg" className='bg-success' onClick={saveEdit} />
                    </div>

                    <div className="mt-3">
                        <label><strong>Name:</strong></label>
                        <input
                            type="text"
                            className="form-control"
                            value={editedName}
                            onChange={(e) => setEditedName(e.target.value)}
                        />
                    </div>

                    <div className="mt-3 mb-3">
                        <label><strong>Level:</strong></label>
                        <input
                            type="number"
                            className="form-control"
                            value={editedLevel}
                            onChange={(e) => setEditedLevel(e.target.value)}
                        />
                    </div>

                    <ItemsAccordion charID={character.id} itemData={itemData} />
                </Card.Body>
            </Card>
        );
    };

    /**
     * sends a request to the db and:
     * on success: updates the states.
     * on failure: shows an error.
     * @param {int} charID - id of the character
     */
    function deleteChar(charID) {
        const token = sessionStorage.getItem('jwt');
        fetch('characters/delete_character', {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                character_id: charID
            })
        })
            .then(res => {
                if (!res.ok) throw new Error('Failed to Delete character');
                return res.json();
            })
            .then(data => {
                setCharData(prevData => ({
                    ...charData,
                    characters: prevData.characters.filter(char => char.id !== charID)
                }));
            })
            .catch(err => {
                setDeleteError('Failed to Delete character')
            });
    }

    /**
     * sends a request to the db to save changes and:
     * on success: updates the states.
     * on failure: shows an error.
     */
    function saveEdit() {
        const token = sessionStorage.getItem('jwt');
        fetch('/characters/update_character', {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                character_id: editingId,
                name: editedName,
                level: parseInt(editedLevel)
            })
        })
            .then(res => {
                if (!res.ok) throw new Error('Failed to update character');
                return res.json();
            })
            .then(data => {
                // Update local charData
                const updatedCharacters = charData.characters.map(char =>
                    char.id === editingId
                        ? { ...char, name: editedName, level: parseInt(editedLevel) }
                        : char
                );
                setCharData({
                    ...charData,
                    characters: updatedCharacters
                });

                setEditingId(null);
            })
            .catch(err => {
                setItemEditError("Item editing failed.");
                setEditingId(null);
            });
    }

    if (itemError || charError || deleteError || itemEditError) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <AlertComponent show='true' onHide={() => { }} msg={itemError || charError || deleteError || itemEditError} />
            </div>
        )
    }

    if (itemLoading || charLoading) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <Spinner animation="border" role="status">
                    <span className="visually-hidden">Loading...</span>
                </Spinner>
            </div>
        );
    }

    return (
        <div className="container-fluid d-flex flex-column" style={{ height: '100vh', overflow: 'hidden' }}>
            <div className="table-responsive flex-grow-1" style={{ overflowY: 'auto' }}>
                <div className="row m-2">
                    <div className="col-auto">
                        <IconButton
                            icon="bi bi-arrow-left-circle-fill"
                            onClick={onBack}
                        />
                    </div>
                </div>

                <div className="row m-2">
                    <Card>
                        <Card.Body>
                            <Card.Title className="text-center">User Info</Card.Title>
                            <Card.Text className="mb-0"><strong>Username:</strong> {user.username}</Card.Text>
                            <Card.Text><strong>Email:</strong> {user.email}</Card.Text>
                        </Card.Body>
                    </Card>

                    {charData.characters?.map((character, index) => renderCard(character, index + 1))}
                </div>
            </div>
        </div>
    )
}

export default UserInfo