import useAPI from "../CustomHooks/useAPI";
import Card from 'react-bootstrap/Card';
import AlertComponent from "./AlertComponent";
import { Spinner } from "react-bootstrap";

const ItemsPage = () => {
    const [itemData, itemLoading, itemError] = useAPI(`http://localhost:5000/inventory/items`, 'failed to fetch items');

    const renderCard = (item, index) => (
        <div key={item.id || index} className="col">
            <Card className="h-100">
                <Card.Img
                    variant="top"
                    src={`data:image/png;base64,${item.icon}`}
                    style={{ height: '200px', objectFit: 'contain' }}
                    alt={item.name}
                />
                <Card.Body>
                    <Card.Title>{item.name}</Card.Title>
                    <Card.Text>{item.description}</Card.Text>
                </Card.Body>
            </Card>
        </div>
    );

    if (itemError) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <AlertComponent show='true' onHide={() => { }} msg={"Fetching data failed."} />
            </div>
        )
    }

    if (itemLoading) {
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
                    <Card>
                        <Card.Body>
                            <Card.Title className="text-center">Items</Card.Title>
                            <Card.Text className="mb-0"><strong>This page displays all game items available in the database.
                                <br /> The Admin cannot affect the items, the stats, availablility or functionality.
                                <br /> Items are fully managed on the Unity side.
                            </strong>
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </div>

                <div className="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4 mt-2">
                    {itemData?.map((item, index) => renderCard(item, index))}
                </div>
            </div>
        </div>
    )
}

export default ItemsPage