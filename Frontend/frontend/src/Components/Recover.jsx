import React, { useState } from 'react';
import Card from 'react-bootstrap/Card';
import { Spinner, Alert, Button } from 'react-bootstrap';
import useAPI from '../CustomHooks/useAPI';

/**
 * A Recover page, to view banned users, and deleted characters.
 * Recover buttons restore users and characters.
 * @returns react component.
 */
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

    /**
     * sends a request to recover a banned user.
     * on success: updates the states.
     * on failure: shows an error.
     * @param {int} userId - id of the user.
     */
    async function recoverUser(userId) {
        setLoadingRecover(true);
        setErrorRecover(null);

        try {
            const token = sessionStorage.getItem('jwt');
            const res = await fetch('/recover', {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
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

    /**
     * sends a request to recover a deleted character.
     * on success: updates the states.
     * on failure: shows an error.
     * @param {int} characterId - id of the character.
     */
    async function recoverCharacter(characterId) {
        setLoadingRecover(true);
        setErrorRecover(null);

        try {
            const token = sessionStorage.getItem('jwt');
            const res = await fetch('/characters/recover_character', {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
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
            <Card>
                <Card.Body>
                    {errorRecover && <Alert variant="danger">{errorRecover}</Alert>}

                    <section className="mb-4">
                        <h5>Deleted Users</h5>
                        {deletedUsers.users && deletedUsers.users.length > 0 ? (
                            deletedUsers.users.map((user) => (
                                <Card key={user.id} className="mb-3">
                                    <Card.Body className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <Card.Title>{user.username}</Card.Title>
                                            <Card.Subtitle className="mb-2 text-muted">{user.email}</Card.Subtitle>
                                        </div>
                                        <Button variant="success" disabled={loadingRecover} onClick={() => recoverUser(user.id)}>
                                            Recover
                                        </Button>
                                    </Card.Body>
                                </Card>
                            ))
                        ) : (
                            <div className="text-muted">No deleted users found.</div>
                        )}
                    </section>

                    <section>
                        <h5>Deleted Characters</h5>
                        {deletedChars.characters && deletedChars.characters.length > 0 ? (
                            deletedChars.characters.map((character) => (
                                <Card key={character.id} className="mb-3">
                                    <Card.Body className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <Card.Title>{character.name}</Card.Title>
                                            <Card.Subtitle className="mb-2 text-muted">Level: {character.level}</Card.Subtitle>
                                            <Card.Subtitle className="mb-2 text-muted">Belongs to: {character.user_name}</Card.Subtitle>
                                        </div>
                                        <Button variant="success" disabled={loadingRecover} onClick={() => recoverCharacter(character.id)}>
                                            Recover
                                        </Button>
                                    </Card.Body>
                                </Card>
                            ))
                        ) : (
                            <div className="text-muted">No deleted characters found.</div>
                        )}
                    </section>
                </Card.Body>
            </Card>
        </div>
    );
};

export default Recover;
