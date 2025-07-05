import useAPI from "../CustomHooks/useAPI";
import Accordion from 'react-bootstrap/Accordion';
import { Spinner } from 'react-bootstrap';

const INVENTORY_SIZE = 20;

const ItemsAccordion = ({ charID, itemData }) => {
    const [data, loading, error] = useAPI(`http://localhost:5000/inventory/${charID}`, 'failed to fetch items');

    const itemMap = {};
    itemData?.forEach(item => {
        itemMap[item.id] = item;
    });

    // Map from slot index to entry for quick lookup
    const inventoryMap = {};
    data?.forEach(entry => {
        if (entry.index != null) {
            inventoryMap[entry.index] = entry;
        }
    });

    function renderSlot(slotIndex) {
        const entry = inventoryMap[slotIndex];
        if (!entry || !entry.item) {
            return (
                <div
                    key={slotIndex}
                    className="col border rounded bg-light d-flex justify-content-center align-items-center m-3"
                    style={{ height: '50px', width: '50px', userSelect: 'none' }}
                >
                    {/* empty slot */}
                </div>
            );
        }

        const fullItem = itemMap[entry.item.id];
        if (!fullItem) {
            // item info missing in itemData
            return (
                <div
                    key={slotIndex}
                    className="col border rounded bg-light d-flex justify-content-center align-items-center m-3"
                    style={{ height: '50px', width: '50px', userSelect: 'none' }}
                >
                    {entry.item.id}
                </div>
            );
        }

        return (
            <div
                key={slotIndex}
                className="col border rounded bg-light d-flex justify-content-center align-items-center m-3"
                style={{ height: '50px', width: '50px', userSelect: 'none' }}
            >
                {fullItem.icon ? (
                    <img
                        src={`data:image/png;base64,${fullItem.icon}`}
                        alt={fullItem.name}
                        style={{ maxHeight: '40px', maxWidth: '40px' }}
                    />
                ) : (
                    fullItem.name
                )}
            </div>
        );
    }

    return (
        <Accordion defaultActiveKey="">
            <Accordion.Item eventKey="0">
                <Accordion.Header>Items</Accordion.Header>
                <Accordion.Body>
                    {loading ? (
                        <div className="d-flex align-items-center">
                            <Spinner animation="border" size="sm" className="me-2" />
                            <span>Loading items...</span>
                        </div>
                    ) : error ? (
                        <p className="text-danger">{error}</p>
                    ) : (
                        <div className="row row-cols-5 g-3">
                            {[...Array(INVENTORY_SIZE).keys()].map(i => renderSlot(i))}
                        </div>
                    )}
                </Accordion.Body>
            </Accordion.Item>
        </Accordion>
    );
}

export default ItemsAccordion;
