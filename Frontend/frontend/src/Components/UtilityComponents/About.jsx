import { Container, Row, Col, Card } from "react-bootstrap";

const authors = [
  {
    name: "Ilya Kirshtein",
    id: "323299362",
    email: "ilyaki@edu.jmc.ac.il"
  },
  {
    name: "Dima Nikonov",
    id: "324656362",
    email: "dimaku@edu.jmc.ac.il"
  }
];

/**
 * A simple About page, showing name, id and emails of the authors.
 * @returns react component
 */
const About = () => {
  return (
    <Container className="py-5">
      <h2 className="text-center mb-5 fw-bold">Authors</h2>
      <Row className="justify-content-center">
        {authors.map((author, index) => (
          <Col key={index} md={5} className="mb-4">
            <Card className="shadow rounded-4 border-0">
              <Card.Body className="p-4">
                <h5 className="fw-bold text-primary mb-3">{author.name}</h5>
                <p className="mb-2">
                  <strong>ID:</strong> {author.id}
                </p>
                <p className="mb-0">
                  <strong>Email: {author.email}</strong>{" "}
                </p>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </Container>
  );
};

export default About;
