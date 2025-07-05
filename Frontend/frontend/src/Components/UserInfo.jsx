import IconButton from "./IconButton";
import Card from 'react-bootstrap/Card';
import useAPI from "../CustomHooks/useAPI";
import ItemsAccordion from "./ItemsAccordion";

const UserInfo = ({ user, onBack }) => {
    const [charData, charLoading, charError] = useAPI(`http://localhost:5000/characters/${user.id}`, 'failed to fetch users characters');
    const [itemData, itemLoading, itemError] = useAPI(`http://localhost:5000/inventory/items`, 'failed to fetch items');


    function renderCard(index, character) {
        return (
            <Card key={character.id || index} className="mt-2">
                <Card.Body>
                    <Card.Title className="text-center">Character #{index}</Card.Title>
                    <Card.Text><strong>Name:</strong> {character.name}</Card.Text>
                    <Card.Text><strong>Level:</strong> {character.level}</Card.Text>
                    <ItemsAccordion charID={character.id} itemData={itemData} />
                </Card.Body>
            </Card>
        );
    };

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
                            <Card.Text><strong>Username:</strong> {user.username}</Card.Text>
                            <Card.Text><strong>Email:</strong> {user.email}</Card.Text>
                        </Card.Body>
                    </Card>

                    {charData.characters?.map((character, index) => renderCard(index, character))}
                </div>
            </div>
        </div>
    )
}

export default UserInfo