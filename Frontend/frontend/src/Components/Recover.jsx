import React, { useState } from 'react';
import Card from 'react-bootstrap/Card';
import { Spinner, Alert, Button } from 'react-bootstrap';
import useAPI from '../CustomHooks/useAPI';

const Recover = () => {
    const [deletedChars, charsLoading, charsError, setDeletedChars] = useAPI(`http://localhost:5000/characters/deleted`, 'Failed to fetch deleted characters');
    const [deletedUsers, usersLoading, usersError, setDeletedUsers] = useAPI('http://localhost:5000/deleted_users', 'Failed to fetch deleted users');
    const [loadingRecover, setLoadingRecover] = useState(false);
    const [errorRecover, setErrorRecover] = useState(null);

    if (charsLoading || usersLoading) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <Spinner animation="border" role="status">
                    <span className="visually-hidden">Loading...</span>
                </Spinner>
            </div>
        );
    }

    if (charsError || usersError) {
        return (
            <div className="container mt-4">
                {charsError && <Alert variant="danger">{charsError}</Alert>}
                {usersError && <Alert variant="danger">{usersError}</Alert>}
            </div>
        );
    }

    async function recoverUser(userId) {
        setLoadingRecover(true);
        setErrorRecover(null);

        try {
            const res = await fetch('/recover', {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ user_id: userId }),
            });
            if (!res.ok) {
                throw new Error('Failed to recover user');
            }

            //update local state to remove recovered user from deleted list
            setDeletedUsers(prev => ({
                ...prev,
                users: prev.users.filter(u => u.id !== userId),
            }));
        } catch (err) {
            setErrorRecover(err.message);
        } finally {
            setLoadingRecover(false);
        }
    }

    async function recoverCharacter(characterId) {
        setLoadingRecover(true);
        setErrorRecover(null);

        try {
            const res = await fetch('/characters/recover_character', {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ character_id: characterId }),
            });
            if (!res.ok) {
                throw new Error('Failed to recover character');
            }
            setDeletedChars(prev => ({
                ...prev,
                characters: prev.characters.filter(c => c.id !== characterId),
            }));
        } catch (err) {
            setErrorRecover(err.message);
        } finally {
            setLoadingRecover(false);
        }
    }

    return (
        <div className="container my-4">
            {errorRecover && <Alert variant="danger">{errorRecover}</Alert>}

            <h2 className="mb-4">Deleted Characters</h2>
            {deletedChars.characters && deletedChars.characters.length > 0 ? (
                deletedChars.characters.map((character) => (
                    <Card key={character.id} className="mb-3">
                        <Card.Body className="d-flex justify-content-between align-items-center">
                            <div>
                                <Card.Title>{character.name}</Card.Title>
                                <Card.Subtitle className="mb-2 text-muted">Level: {character.level}</Card.Subtitle>
                            </div>
                            <Button
                                variant="success"
                                disabled={loadingRecover}
                                onClick={() => recoverCharacter(character.id)}
                            >
                                Recover
                            </Button>
                        </Card.Body>
                    </Card>
                ))
            ) : (
                <p>No deleted characters found.</p>
            )}

            <h2 className="mb-4 mt-5">Deleted Users</h2>
            {deletedUsers.users && deletedUsers.users.length > 0 ? (
                deletedUsers.users.map((user) => (
                    <Card key={user.id} className="mb-3">
                        <Card.Body className="d-flex justify-content-between align-items-center">
                            <div>
                                <Card.Title>{user.username}</Card.Title>
                                <Card.Subtitle className="mb-2 text-muted">{user.email}</Card.Subtitle>
                            </div>
                            <Button
                                variant="success"
                                disabled={loadingRecover}
                                onClick={() => recoverUser(user.id)}
                            >
                                Recover
                            </Button>
                        </Card.Body>
                    </Card>
                ))
            ) : (
                <p>No deleted users found.</p>
            )}
        </div>
    );
};

export default Recover;
