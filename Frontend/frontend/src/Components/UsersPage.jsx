import Spinner from 'react-bootstrap/Spinner';
import useAPI from '../CustomHooks/useAPI'
import Table from 'react-bootstrap/Table';
import IconButton from './IconButton';
import AlertComponent from './AlertComponent';
import { useState } from 'react';
import UserInfo from './UserInfo';

const UsersPage = () => {
    const [data, loading, error, setData] = useAPI('http://localhost:5000/users', 'failed to fetch users');
    const [showUserInfo, setShowUserInfo] = useState(false);
    const [selectedUserIndex, setSelectedUserIndex] = useState(null);
    const [deleteError, setDeleteError] = useState(null);

    function renderTable(user, index) {
        return (
            <tr key={index}>
                <td>{index}</td>
                <td>{user.username}</td>
                <td>{user.email}</td>
                <td>
                    <div className="d-flex gap-2 justify-content-center">
                        <IconButton icon="bi bi-trash-fill" className='bg-danger' onClick={() => deleteUser(user.id)} />
                        <IconButton icon="bi bi-info-circle-fill" className='bg-primary' onClick={() => {
                            setSelectedUserIndex(index);
                            setShowUserInfo(true);
                        }} />
                    </div>
                </td>
                <td>
                    <div className="d-flex gap-2 justify-content-center">
                        <IconButton icon="bi bi-download" className='bg-warning' onClick={() => { }} />
                    </div>
                </td>
            </tr>);
    }

    function deleteUser(userID) {
        fetch('/delete', {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                user_id: userID
            })
        })
            .then(res => {
                if (!res.ok) throw new Error('Failed to Delete user');
                return res.json();
            })
            .then(data => {
                if (data.success) {
                    setData(prevData => ({
                        ...prevData,
                        users: prevData.users.filter(user => user.id !== userID)
                    }));
                }
            })
            .catch(err => {
                setDeleteError("Failed to delete user")
            });
    }

    if (error || deleteError) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <AlertComponent show='true' onHide={() => { }} msg={error || deleteError} />
            </div>
        )
    }

    if (loading) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <Spinner animation="border" role="status">
                    <span className="visually-hidden">Loading...</span>
                </Spinner>
            </div>
        );
    }

    if (showUserInfo) {
        return (
            <UserInfo user={data.users[selectedUserIndex]} onBack={() => setShowUserInfo(false)} />
        )
    }

    return (
        <div className="container-fluid d-flex flex-column" style={{ height: '100vh', overflow: 'hidden' }}>
            <div className="table-responsive flex-grow-1" style={{ overflowY: 'auto' }}>
                <Table striped bordered hover size="sm" className="mt-2 text-center">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Username</th>
                            <th>Email</th>
                            <th>Actions</th>
                            <th>Statistics</th>
                        </tr>
                    </thead>
                    <tbody>
                        {data.users?.map((user, index) => renderTable(user, index))}
                    </tbody>
                </Table>
            </div>
        </div>
    );

}

export default UsersPage