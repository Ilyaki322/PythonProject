
/**
 * A modular button to also include a bootstrap icon.
 * @param {string} icon - bootstrap icon css class, eg: 'bi bi-trash-fill' or 'bi bi-check-lg'.
 * @param {function} onClick - function to call on click.
 * @param {string} className - additional css classes to add.
 * @returns react component.
 */
const IconButton = ({ icon, onClick, className = '' }) => {
    return (
        <button
            type="button"
            className={`btn btn-light btn-sm d-flex align-items-center justify-content-center ${className}`}
            onClick={onClick}
        >
            <i className={`${icon}`}></i>
        </button>
    );
};

export default IconButton