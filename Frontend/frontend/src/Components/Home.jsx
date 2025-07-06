import About from './About';
import { Card } from 'react-bootstrap';

const Home = () => {
    return (
        <div className="container-fluid d-flex flex-column mt-5" style={{ height: '100vh', overflow: 'hidden' }}>
            <div className="row m-2">
                <Card>
                    <Card.Body>
                        <Card.Text className="mb-0"><strong>This site represents a database 'frontend' letting Admins see and control some aspects of the games database.
                            <br /> Users Page: Lets you ban users, download statistics and see their characters.
                            <br /> Users Info Page: For each character you can edit its level and name, delete it and modify in game inventory. (inventory slots are clickable!)
                            <br /> Items Page: Lets you see all items available in Game.
                            <br /> Recover Page: Lets you unban users and recover deleted characters (deleted both by the player himself and by admins).
                        </strong>
                        </Card.Text>
                    </Card.Body>
                </Card>
            </div>

            <div className="row m-2">
                <About />
            </div>

        </div>
    )
}

export default Home