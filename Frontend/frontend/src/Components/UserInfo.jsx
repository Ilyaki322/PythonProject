import IconButton from "./IconButton";
import Card from 'react-bootstrap/Card';
import useAPI from "../CustomHooks/useAPI";
import ItemsAccordion from "./ItemsAccordion";
import AlertComponent from "./AlertComponent";
import { Spinner } from "react-bootstrap";
import { useState } from "react";

const UserInfo = ({ user, onBack }) => {
    const [charData, charLoading, charError, setCharData] = useAPI(`http://localhost:5000/characters/${user.id}`, 'failed to fetch users characters');
    const [itemData, itemLoading, itemError] = useAPI(`http://localhost:5000/inventory/items`, 'failed to fetch items');

    const [editingId, setEditingId] = useState(null);
    const [editedName, setEditedName] = useState('');
    const [editedLevel, setEditedLevel] = useState(0);

    function renderCard(character) {
        if (character.id === editingId) return renderEditCard(character)
        return (
            <Card key={character.id} className="mt-2">
                <Card.Body>
                    <Card.Title className="text-center">Character #{character.id}</Card.Title>

                    <div className="d-flex gap-2">
                        <IconButton icon="bi bi-trash-fill" className='bg-danger' Click={() => { }} />
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

    function renderEditCard(character) {
        return (
            <Card key={character.id} className="mt-2 bg-warning">
                <Card.Body>
                    <Card.Title className="text-center">Character #{character.id}</Card.Title>

                    <div className="d-flex gap-2">
                        <IconButton icon="bi bi-trash-fill" className='bg-danger' Click={() => { }} />
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

    function saveEdit() {
        fetch('/characters/update_character', {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                character_id: editingId,
                name: editedName,
                level: parseInt(editedLevel)  // ensure number type
            })
        })
            .then(res => {
                if (!res.ok) throw new Error('Failed to update character');
                return res.json();
            })
            .then(data => {
                if (data.success) {
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
                }

                setEditingId(null);
            })
            .catch(err => {
                console.error('Error updating character:', err);
                setEditingId(null);
            });
    }

    if (itemError || charError) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <AlertComponent show='true' onHide={() => { }} msg={"Fetching data failed."} />
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

                    {charData.characters?.map((character) => renderCard(character))}
                </div>
            </div>
        </div>
    )
}

export default UserInfo