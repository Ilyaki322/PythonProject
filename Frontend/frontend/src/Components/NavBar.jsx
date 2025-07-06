import { Link, Outlet } from 'react-router';
import Container from 'react-bootstrap/Container';
import { Row, Col, Button } from 'react-bootstrap';

/**
 * Sidebar, using react-router to easily traverse pages
 * @returns React component
 */
const NavBar = () => {
  return (
    <Container fluid className='bg-secondary'>
      <Row>
        <Col className="bg-dark text-white pt-3" style={{ height: '100vh' }} >
          <Row className="justify-center mb-2">
            <div className="text-center">
              <h3>Admin</h3>
            </div>
          </Row>
          <div className="d-flex flex-column">
            <Button as={Link} to="/" variant="outline-light" className="mb-3">
              Users
            </Button>
            <Button as={Link} to="/Items" variant="outline-light" className="mb-3">
              Items
            </Button>
            <Button as={Link} to="/About" variant="outline-light" className="mb-3">
              About
            </Button>
          </div>
        </Col>
        <Col xs={10}>
          <Outlet />
        </Col>
      </Row >
    </Container >
  )
};

export default NavBar;
