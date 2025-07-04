import Button from 'react-bootstrap/Button';
import Spinner from 'react-bootstrap/Spinner';

/**
 * A generic Spinner + Button.
 * @param {enabled} a flag that indicates whether this acts as a button or a spinner.
 * @param {onClick} function to call when the button is clicked.
 * @returns react component
 */
const SpinnerButton = ({ enabled, onClick }) => {
  return (
    <Button variant="primary" disabled={!enabled} onClick={onClick}>
      {!enabled && (
        <Spinner
          as="span"
          animation="border"
          size="sm"
          role="status"
          aria-hidden="true"
          className="me-2"
        />
      )}
      {'Load Countries'}
    </Button>
  );
}

export default SpinnerButton