import useAPI from '../CustomHooks/useAPI'
import { useState } from 'react';

const UsersPage = () => {

    const [data, loading, error, setData, refetch] = useAPI('http://localhost:5000/characters', 'failed to fetch countries');
    const [show, setShow] = useState(false);

    return (
        <div>mainPage</div>
    )
}

export default UsersPage