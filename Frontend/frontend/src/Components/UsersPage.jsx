import { useState } from 'react';
import useAPI from '../CustomHooks/useAPI'
import UserAccordion from './UserAccordion';

const UsersPage = () => {

    const [data, loading, error, setData, refetch] = useAPI('http://localhost:5000/users', 'failed to fetch users');
    const [show, setShow] = useState(false);

    return (
        <div>
            {data.users?.map((user, index) => (
                <UserAccordion key={user.id} username={user.username} email={user.email} userID={user.id} />
            ))}
        </div>
    );
}

export default UsersPage