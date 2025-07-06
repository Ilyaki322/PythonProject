import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import DropDown from './UtilityComponents/DropDown';
import { useState, useEffect } from 'react';

/**
 * A bootstrap modal to edit Item properties.
 * @param {bool} show - show/hide the modal.
 * @param {function} setShow - set function for show/hide. 
 * @param {object array} items - list of items fetched from db.
 * @param {string} selected - the item name we want to edit.
 * @param {function} onSave - call back to save results. 
 * @returns react component.
 */
const ItemModal = ({ show, setShow, items, selected, onSave }) => {
    const handleClose = () => setShow(false);

    // call onSave, hide the modal.
    function saveChanges() {
        onSave({ name: newSelect, count: newCount, index: selected.index })
        setShow(false);
    }

    const itemsArray = ['Empty'];
    items?.forEach((item) => itemsArray.push(item.name));

    const [newSelect, setNewSelect] = useState(selected?.name || '');
    const [newCount, setNewCount] = useState(selected?.count || '');

    useEffect(() => {
        setNewSelect(selected?.name || '');
        setNewCount(selected?.count || '');
    }, [selected]);

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Edit Item</Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <label htmlFor="item-select"><strong>Item:</strong></label>
                <DropDown selectedVal={selected?.name}
                    id='select-item'
                    onSelect={(val) => setNewSelect(val)}
                    options={itemsArray} />

                <label htmlFor="item-count" className="mt-3 d-block"><strong>Count:</strong></label>
                <input id="item-count" type='number' value={newCount} onChange={(e) => setNewCount(e.target.value)} />
            </Modal.Body>

            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Close
                </Button>
                <Button variant="primary" onClick={saveChanges}>
                    Save Changes
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export default ItemModal