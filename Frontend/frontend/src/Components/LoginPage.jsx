import React, { useState } from 'react';
import { useAuth } from './AuthContext'
import { useNavigate } from 'react-router';

/**
 * Login page, inorder to gain jwt token and get access to other pages.
 * @returns react component.
 */
function LoginPage() {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    /**
     * handle login click.
     * on success forwards us to home page.
     * shows an error on failure.
     * @param {*} e - event
     */
    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const res = await fetch('/login_admin', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password })
            });

            const data = await res.json();

            if (res.ok) {
                login(data.token); // save JWT
                navigate('/'); // redirect
            } else {
                setError(data.message || 'Login failed');
            }
        } catch (err) {
            setError('Error connecting to server');
        }
    };

    return (
        <div className="container-fluid d-flex justify-content-center align-items-center bg-dark" style={{ height: '100vh' }}>
            <div className="card p-4 shadow" style={{ width: '100%', maxWidth: '400px' }}>
                <h3 className="text-center mb-4">Login</h3>
                {error && <div className="alert alert-danger">{error}</div>}
                <form onSubmit={handleLogin}>
                    <div className="mb-3">
                        <label htmlFor="username" className="form-label">Username</label>
                        <input
                            type="text"
                            id="username"
                            className="form-control"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <label htmlFor="password" className="form-label">Password</label>
                        <input
                            type="password"
                            id="password"
                            className="form-control"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    <button type="submit" className="btn btn-primary w-100">Login</button>
                </form>
            </div>
        </div>
    );
};

export default LoginPage;
