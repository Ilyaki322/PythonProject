import Spinner from 'react-bootstrap/Spinner';
import useAPI from '../CustomHooks/useAPI'
import Table from 'react-bootstrap/Table';
import IconButton from './UtilityComponents/IconButton';
import AlertComponent from './UtilityComponents/AlertComponent';
import { useState } from 'react';
import UserInfo from './UserInfo';

/**
 * This page fetches all not deleted users, and shows a table with an option to:
 * 1. delete the user.
 * 2. see more info.
 * 3. download statistics about him.
 * On clicking the info buttons takes us to UserInfo component.
 * @returns react component
 */
const UsersPage = () => {
    const [data, loading, error, setData] = useAPI('http://localhost:5000/users', 'failed to fetch users');
    const [showUserInfo, setShowUserInfo] = useState(false);
    const [selectedUserIndex, setSelectedUserIndex] = useState(null);
    const [deleteError, setDeleteError] = useState(null);
    const [excelError, setExcelError] = useState(null);

    /**
     * Renders a single bootstrap table entry.
     * @param {object} user a single user, as gotten from the db.
     * @param {int} index - index of the user in the loop.
     * @returns html/jsx piece.
     */
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
                        <IconButton icon="bi bi-download" className='bg-warning' onClick={() => downloadUserStats(user.id)} />
                    </div>
                </td>
            </tr>);
    }

    /**
     * sends a request to the backend and:
     * success: updates the states.
     * failure: shows an error.
     * @param {int} userID - id of the user
     */
    function deleteUser(userID) {
        const token = sessionStorage.getItem('jwt');
        fetch('/delete', {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
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
                    setData(prevData => prevData.filter(user => user.id !== userID));
                }
            })
            .catch(err => {
                setDeleteError("Failed to delete user")
            });
    }

    /**
     * downloads an excel file with user stats on it
     * @param {int} userId - id of the user
     */
    function downloadUserStats(userId) {
        const token = sessionStorage.getItem('jwt');

        fetch(`http://localhost:5000/user_data/${userId}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to download file');
                }
                return response.blob();
            })
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `user_${userId}_matches.xlsx`;
                document.body.appendChild(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            })
            .catch(error => {
                setExcelError("Failed to download data");
            });
    }

    if (error || deleteError || excelError) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <AlertComponent show='true' onHide={() => { }} msg={error || deleteError || excelError} />
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
            <UserInfo user={data[selectedUserIndex]} onBack={() => setShowUserInfo(false)} />
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
                        {data?.map((user, index) => renderTable(user, index))}
                    </tbody>
                </Table>
            </div>
        </div>
    );

}

export default UsersPage