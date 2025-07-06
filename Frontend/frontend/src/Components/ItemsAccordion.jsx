import useAPI from "../CustomHooks/useAPI";
import Accordion from 'react-bootstrap/Accordion';
import { Spinner } from 'react-bootstrap';
import ItemModal from "./ItemModal";
import { useState } from "react";

const INVENTORY_SIZE = 20;

const ItemsAccordion = ({ charID, itemData }) => {
    const [data, loading, error, setData] = useAPI(`http://localhost:5000/inventory/${charID}`, 'failed to fetch items');
    const [showModal, setShowModal] = useState(false);
    const [selected, setSelected] = useState(null);

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

    function onSaveItem(newEntry) {
        const item = itemData?.find(item => item.name === newEntry.name);
        const itemID = item ? item.id : null

        let updated = false;
        const newData = data.map(entry => {
            if (entry.index === newEntry.index) {
                updated = true;
                return {
                    item: item,
                    count: newEntry.count,
                    index: newEntry.index
                };
            }
            return entry;
        });

        if (!updated) {
            newData.push({
                item: item,
                count: newEntry.count,
                index: newEntry.index
            });
        }

        fetch('/inventory/update_slot', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                character_id: charID,
                item_id: itemID,
                count: newEntry.count,
                index: newEntry.index
            })
        });
        setData(newData);
    }

    function renderSlot(slotIndex) {
        const entry = inventoryMap[slotIndex];
        const isEmpty = !entry || !entry.item;
        const fullItem = isEmpty ? null : itemMap[entry.item.id];
        const hasFullItemInfo = fullItem !== null;

        let content;
        if (isEmpty) {
            content = null; // Empty slot, no content needed beyond the styling
        } else if (hasFullItemInfo && fullItem.icon) {
            content = (
                <img
                    src={`data:image/png;base64,${fullItem.icon}`}
                    alt={fullItem.name}
                    style={{ maxHeight: '40px', maxWidth: '40px' }}
                />
            );
        } else if (hasFullItemInfo) {
            content = fullItem.name;
        } else {
            content = entry.item.id; // Fallback if item info is missing
        }

        return (
            <div
                key={slotIndex}
                className="col border rounded bg-light d-flex justify-content-center align-items-center m-3 position-relative"
                style={{ height: '50px', width: '50px', userSelect: 'none' }}
                onClick={() => {
                    if (isEmpty) setSelected({ name: 'Empty', count: 0, index: slotIndex });
                    else setSelected({ name: fullItem.name, count: inventoryMap[slotIndex].count, index: slotIndex })
                    setShowModal(true)
                }}
            >
                {content}
                {!isEmpty && entry.count > 1 && ( // Only show count if it's an item and count > 1
                    <span
                        className="position-absolute top-0 end-0 bg-dark text-white rounded-circle d-flex justify-content-center align-items-center"
                        style={{
                            fontSize: '0.7rem',
                            width: '20px',
                            height: '20px',
                            transform: 'translate(25%, -25%)',
                            zIndex: 1
                        }}
                    >
                        {entry.count}
                    </span>
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

            <ItemModal setShow={setShowModal} show={showModal} items={itemData} selected={selected} onSave={onSaveItem} />
        </Accordion>
    );
}

export default ItemsAccordion;
