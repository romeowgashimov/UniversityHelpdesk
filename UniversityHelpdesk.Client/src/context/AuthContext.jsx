import React, { createContext, useState, useEffect, useContext } from "react";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem("jwt_token");
        const role = localStorage.getItem("user_role");
        const name = localStorage.getItem("user_name");

        if(token && role && name){
            setUser({ token, role, name });
        }
        setLoading(false);
    }, []);

    const login = (userData) => {
        localStorage.setItem("jwt_token", userData.token);
        localStorage.setItem("user_role", userData.role)
        localStorage.setItem("user_name", userData.fullName);

        setUser({
            token: userData.token,
            role: userData.role,
            name: userData.fullName
        });
    };

    const logout = () => {
        localStorage.clear();
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, logout, isAuthenticated: !!user, loading}}>
            {!loading && children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);