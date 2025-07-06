import { Navigate, Outlet } from 'react-router';
import { useAuth } from './AuthContext';


/**
 * ProtectedRoute component to guard routes that require authentication.
 * 
 * This component checks if a user is authenticated by accessing the `token`
 * from the AuthContext. If the user is authenticated (i.e., token exists),
 * it renders the nested routes via <Outlet />.
 * 
 * If the user is not authenticated, it redirects them to the "/login" page.
 * 
 * Usage:
 * Wrap this component around any route that should be protected.
 * 
 * Example with react-router v6:
 * 
 * <Route element={<ProtectedRoute />}>
 *   <Route path="/dashboard" element={<Dashboard />} />
 * </Route>
 * 
 * @returns {JSX.Element} The nested routes if authenticated, otherwise a redirect to login.
 */
const ProtectedRoute = () => {
    const { token } = useAuth();
    return token ? <Outlet /> : <Navigate to="/login" />;
};

export default ProtectedRoute;
