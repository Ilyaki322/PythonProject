

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