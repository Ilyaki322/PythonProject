import { createContext, useState, useContext } from "react";

const AuthContext = createContext();

/**
 * AuthProvider component provides authentication context to its children.
 * It manages a JWT token stored in sessionStorage and exposes login and logout functions.
 * 
 * @param {Object} props - React component props.
 * @param {React.ReactNode} props.children - React children nodes that will have access to auth context.
 * 
 * @returns {JSX.Element} The AuthContext.Provider wrapping children with authentication context.
 * 
 * @example
 * // Wrap your app or part of your app with AuthProvider
 * <AuthProvider>
 *   <App />
 * </AuthProvider>
 * 
 * // Inside components, use the useAuth hook to access login/logout and token
 * const { token, login, logout } = useAuth();
 */
export const AuthProvider = ({ children }) => {
    const [token, setToken] = useState(sessionStorage.getItem('jwt') || null);

    const login = (jwt) => {
        sessionStorage.setItem('jwt', jwt);
        setToken(jwt);
    };

    const logout = () => {
        sessionStorage.removeItem('jwt');
        setToken(null);
    };

    return (
        <AuthContext.Provider value={{ token, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

/**
 * useAuth hook allows components to consume the authentication context.
 * 
 * @returns {Object} An object containing:
 * - token: The current JWT token or null if not logged in.
 * - login: Function to save a JWT token and update the context.
 * - logout: Function to clear the JWT token and update the context.
 * 
 * @example
 * const { token, login, logout } = useAuth();
 */
export const useAuth = () => useContext(AuthContext);
