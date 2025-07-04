import { Form, Card } from 'react-bootstrap';


export default function PlayerForm({ formData, categories, onChange }) {
    return (
        <Card className="mb-5 shadow-sm border-secondary bg-secondary bg-opacity-25">
            <Card.Body className="p-4">
                <Form>
                    <Form.Group className="mb-3" controlId="nickname">
                        <Form.Label>Nickname</Form.Label>
                        <Form.Control
                            type="text"
                            name="nickname"
                            placeholder="Enter your nickname"
                            value={formData.nickname}
                            onChange={onChange}
                        />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="category">
                        <Form.Label>Category</Form.Label>
                        <DropDown
                            selectedVal={formData.category || 'Choose a category'}
                            options={categories}
                            onSelect={val => onChange({ target: { name: 'category', value: val } })}
                        />
                    </Form.Group>
                </Form>
            </Card.Body>
        </Card>
    );
}
