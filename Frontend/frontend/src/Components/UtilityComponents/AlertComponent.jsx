import Alert from 'react-bootstrap/Alert';

/**
 * A simple bootstrap alert component
 * @param {bool} show - turn on/off the component. 
 * @param {function} onHide - manages safe turnoff for the component.
 * @param {String} msg - the error to show.
 * @returns react component
 */
const AlertComponent = ({ show, onHide, msg }) => {
    return (
        <Alert variant="danger" onClose={onHide} show={show} dismissible>
            <Alert.Heading>You got an error!</Alert.Heading>
            <p>
                {msg}
            </p>
        </Alert>
    );
}

export default AlertComponent