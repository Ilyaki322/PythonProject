import { useState } from 'react';
import Accordion from 'react-bootstrap/Accordion';
import { Spinner } from 'react-bootstrap';

const UserAccordion = ({ username, email, userID }) => {
    const [showSpinner, setShowSpinner] = useState(false);
    const [charactersData, setCharacters] = useState([]);
    const [isFetched, setIsFetched] = useState(false);

    const handleFetch = async () => {
        if (isFetched) return;

        setShowSpinner(true);
        try {
            const res = await fetch(`http://localhost:5000/characters/${userID}`);
            if (!res.ok) throw new Error(`Fetch failed: ${res.status}`);
            const json = await res.json();
            setCharacters(json);
            setIsFetched(true);
        } catch (error) {
            console.error('Error fetching characters:', error); // change later!!!
        } finally {
            setShowSpinner(false);
        }
    };

    return (
        <Accordion defaultActiveKey="">
            <Accordion.Item eventKey="0">
                <Accordion.Header onClick={handleFetch}>
                    {username}
                </Accordion.Header>
                <Accordion.Body>
                    {showSpinner ? (
                        <div className="d-flex align-items-center">
                            <Spinner animation="border" size="sm" className="me-2" />
                            <span>Loading characters...</span>
                        </div>
                    ) : (
                        <div>
                            <p><strong>Username:</strong> {username}</p>
                            <p><strong>Email:</strong> {email}</p>
                            <p><strong>Characters:</strong></p>
                            <ul>
                                {charactersData.characters?.map((char) => (
                                    <li key={char.id}>{char.name}</li>
                                ))}
                            </ul>
                        </div>
                    )}
                </Accordion.Body>
            </Accordion.Item>
        </Accordion>
    );
}

export default UserAccordion