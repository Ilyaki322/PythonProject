import React, { useState } from 'react';
import Dropdown from 'react-bootstrap/Dropdown';

/**
 * DropDown renders a Bootstrap dropdown button with a scrollable list of options.
 * It maintains the currently selected item in its own state and notifies
 * the parent via a callback when a new item is chosen.
 *
 * @param {object} props
 * @param {string} props.selectedVal            - Initial label shown on the toggle button.
 * @param {string[]} props.options              - Array of option strings to display in the menu.
 * @param {(selected: string) => void} props.onSelect - Callback invoked with the newly selected string.
 * @returns The rendered Dropdown component.
 */
const DropDown = ({ selectedVal, options, onSelect }) => {

    const [selected, setSelected] = useState(selectedVal);

    /**
     * Handles a selection event for the dropdown.
     * @param eventKey is the selected item. 
     */
    const handleSelect = (eventKey) => {
        setSelected(eventKey);
        onSelect(eventKey);
    }

    return (
        <Dropdown onSelect={handleSelect}>
            <Dropdown.Toggle variant="secondary">
                {selected}
            </Dropdown.Toggle>

            <Dropdown.Menu className="scrollable-menu w-100"
                style={{
                    maxHeight: '200px',
                    overflowY: 'auto'
                }}
            >
                {options.map((item, index) => (
                    <Dropdown.Item eventKey={item} key={index}>{item}</Dropdown.Item>
                ))}
            </Dropdown.Menu>
        </Dropdown>
    );
}

export default DropDown 