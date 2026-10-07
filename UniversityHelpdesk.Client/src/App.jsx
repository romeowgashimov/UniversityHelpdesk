import React, { useState } from "react";
import { useAuth } from "./context/AuthContext";
import Login from "./pages/Login/Login";
import TicketList from "./pages/TicketList/TicketList";
import CreateTicket from "./pages/CreateTicket/CreateTicket";

function App() {
    const { isAuthenticated, user, logout } = useAuth();

    const [refreshTrigger, setRefreshTrigger] = useState(0);
    const handleTicketCreated = () => setRefreshTrigger(prev => prev + 1);

    return (
        <div className="container my-5>">
            {isAuthenticated ? (
                <div className="card shadow-sm p-4">
                    <div className="d-flex justify-content-between align-items-center mb-4 border-bottom pb-3">
                        <div>
                            <h2 className="text-primary fw-bold mb-0">Панель управления заявками</h2>
                            <span className="text-muted small">Добро пожаловать, {user.name} ({user.role})</span>    
                        </div>
                        <button className="btn btn-outline-danger btn-sm" onClick={logout}>Выйти</button>
                    </div>

                    <div className="row g-4">
                        {user.role === 'Applicant' && (
                            <div className="col-12">
                                <CreateTicket onTicketCreated={handleTicketCreated}></CreateTicket>
                            </div>
                        )}

                        <div className="col-12">
                            <TicketList key={refreshTrigger}/>
                        </div>
                    </div>
                </div>
            ) : (
                <Login />
            )}
        </div>
    );
}

export default App;