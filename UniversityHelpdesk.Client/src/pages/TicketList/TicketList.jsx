import React, { useState, useEffect } from "react";
import { useAuth } from "../../context/AuthContext";
import TicketDetailsModal from "./TicketDetailsModal";
import { ticketService } from "../../services/ticketService";

function TicketList() {
    const [tickets, setTickets] = useState([]);
    const [loading, setloading] = useState(true);
    const [error, setError] = useState("");
    const { user } = useAuth();
    const [selectedTicket, setSelectedTicket] = useState(null);

    const [filters, setFilters] = useState({
        status: "",         
        equipmentType: "",  
        fromDate: "",       
        toDate: "",         
        isAscending: false,
        showOnlyMine: false
    });

    const loadTickets = async () => {
        setloading(true);
        setError("");
        try {
            const cleanFilters = {
                status: filters.status === "" ? null : parseInt(filters.status),
                equipmentType: filters.equipmentType === "" ? null : parseInt(filters.equipmentType),
                fromDate: filters.fromDate === "" ? null : filters.fromDate,
                toDate: filters.toDate === "" ? null : filters.toDate,
                isAscending: filters.isAscending,
                showOnlyMine: filters.showOnlyMine
            };

            // Передаем фильтры в API 🚀
            const data = await ticketService.getMyTickets(cleanFilters);
            
            if (Array.isArray(data)) {
                setTickets(data);
            } else {
                throw new Error(data.message || "Сервер вернул некорректный формат данных");
            }
        } catch (err) {
            setError(err.message || "Не удалось загрузить список заявок");
            setTickets([]); 
        } finally {
            setloading(false);
        }
    };

    useEffect(() => {
        loadTickets();
    }, [filters]);

    const handleAssign = async (id) => {
        await ticketService.assign(id);
        loadTickets();
    };

    const handleComplete = async (id) => {
        await ticketService.complete(id);
        loadTickets();
    };

    const handleCancel = async (id) => {
        await ticketService.cancel(id);
        loadTickets();
    };

    const handleFilterChange = (e) => {
        const { name, value } = e.target;
        setFilters(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const getStatusBadge = (status) => {
        switch (status) {
            case 1: return <span className="badge bg-info text-dark">Новая</span>;
            case 2: return <span className="badge bg-warning text-dark">В работе</span>;
            case 3: return <span className="badge bg-success">Выполнена</span>;
            case 4: return <span className="badge bg-secondary">Отменена</span>;
            default: return <span className="badge bg-light text-dark">Неизвестно</span>;
        }
    };

    const getEquipmentTypeName = (type) => {
        const types = { 1: "Компьютер", 2: "Принтер/Сканер", 3: "Проектор/Экран", 4: "Сетевое устройство", 5: "Аудиооборудование", 6: "Другое" };
        return types[type] || "Неизвестно";
    };

    return (
        <div className="mt-4">
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h4 className="fw-bold text-secondary">Список моих заявок</h4>
                <button className="btn btn-sm btn-outline-primary" onClick={loadTickets}>Обновить</button>
            </div>

            {error && <div className="alert alert-danger">{error}</div>}

            {/* 3. 🎨 ВИЗУАЛЬНАЯ ПАНЕЛЬ ФИЛЬТРОВ BOOTSTRAP */}
            <div className="card shadow-sm border-0 mb-4 bg-white p-3">
                <div className="row g-3 align-items-end">
                    {/* Выбор статуса */}
                    <div className="col-md-3 col-sm-6">
                        <label className="form-label small fw-semibold text-muted">Статус</label>
                        <select className="form-select form-select-sm" name="status" value={filters.status} onChange={handleFilterChange}>
                            <option value="">Все статусы</option>
                            <option value="1">Новая</option>
                            <option value="2">В работе</option>
                            <option value="3">Выполнена</option>
                            <option value="4">Отменена</option>
                        </select>
                    </div>

                    {/* Выбор типа техники */}
                    <div className="col-md-3 col-sm-6">
                        <label className="form-label small fw-semibold text-muted">Тип оборудования</label>
                        <select className="form-select form-select-sm" name="equipmentType" value={filters.equipmentType} onChange={handleFilterChange}>
                            <option value="">Все типы</option>
                            <option value="1">Компьютер</option>
                            <option value="2">Принтер/Сканер</option>
                            <option value="3">Проектор/Экран</option>
                            <option value="4">Сетевое устройство</option>
                            <option value="5">Аудиооборудование</option>
                            <option value="6">Другое</option>
                        </select>
                    </div>

                    {/* Выбор даты ОТ */}
                    <div className="col-md-2 col-sm-6">
                        <label className="form-label small fw-semibold text-muted">Дата с</label>
                        <input type="date" className="form-control form-control-sm" name="fromDate" value={filters.fromDate} onChange={handleFilterChange} />
                    </div>

                    {/* Выбор даты ДО */}
                    <div className="col-md-2 col-sm-6">
                        <label className="form-label small fw-semibold text-muted">Дата по</label>
                        <input type="date" className="form-control form-control-sm" name="toDate" value={filters.toDate} onChange={handleFilterChange} />
                    </div>

                    {/* Тумблер сортировки */}
                    <div className="col-md-2 col-sm-12 text-md-end text-start mb-2">
                        <div className="form-check form-switch d-inline-block">
                            <input 
                                className="form-check-input" 
                                type="checkbox" 
                                role="switch" 
                                id="sortSwitch" 
                                name="isAscending" 
                                checked={filters.isAscending} 
                                onChange={() => setFilters(prev => ({ ...prev, isAscending: !prev.isAscending }))} 
                            />
                            <label className="form-check-label small fw-semibold text-muted ms-1" htmlFor="sortSwitch">
                                Старые сначала
                            </label> 
                        </div>
                    </div>

                    {user.role === 'Technician' && (
                        <div className="col-md-2 col-sm-6 text-start mb-2">
                            <div className="form-check form-switch d-inline-block">
                                <input 
                                    className="form-check-input" 
                                    type="checkbox" 
                                    role="switch" 
                                    id="mineSwitch" 
                                    checked={filters.showOnlyMine} 
                                    onChange={() => setFilters(prev => ({ ...prev, showOnlyMine: !prev.showOnlyMine }))} 
                                />
                                <label className="form-check-label small fw-semibold text-muted ms-1" htmlFor="mineSwitch">
                                    Только мои
                                </label>
                            </div>
                        </div>
                    )}

                </div>
            </div>

            {/* РЕНДЕР ТАБЛИЦЫ ИЛИ ПЛАШКИ ОТСУТСТВИЯ */}
            {loading ? (
                <div className="text-center my-5">
                    <div className="spinner-border text-primary" role="status"></div>
                </div>
            ) : tickets.length === 0 ? (
                <div className="text-center p-5 bg-white rounded shadow-sm border">
                    <span className="fs-1">📭</span>
                    <p className="text-muted mt-2 mb-0">У вас пока нет заявок в выбранной категории</p>
                </div>
            ) : (
                <div className="table-responsive shadow-sm rounded border bg-white">
                    <table className="table table-hover align-middle mb-0">
                        <thead className="table-light">
                            <tr>
                                <th>Идентификатор</th>
                                <th>Дата</th>
                                <th>Тип техники</th>
                                <th>Детали / Кабинет</th>
                                <th>Описание проблемы</th>
                                <th>Статус</th>
                                <th>Исполнитель</th>
                            </tr>
                        </thead>
                        <tbody>
                            {tickets.map((ticket) => (
                                <tr key={ticket.id}>
                                    <td className="fw-bold text-muted">#{ticket.id}</td>
                                    <td className="small">{new Date(ticket.createdAt).toLocaleDateString("ru-RU")}</td>
                                    <td>{getEquipmentTypeName(ticket.equipmentType)}</td>
                                    <td>
                                        <div className="fw-semibold">{ticket.roomNumber}</div>
                                        <div className="text-muted small text-truncate" style={{ maxWidth: "200px" }}>
                                            {ticket.equipmentDetails}
                                        </div>
                                    </td>

                                    <td className="small text-wrap text-break" style={{ maxWidth: "200px" }}>
                                    {ticket.description.length > 30 
                                        ? `${ticket.description.substring(0, 30)}...` 
                                        : ticket.description}
                                    </td>

                                    <td>{getStatusBadge(ticket.status)}</td>
                                    <td>
                                        {user.role === 'Technician' ? (
                                            <div className="d-flex align-items-center gap-2">
                                                {ticket.status === 1 && (
                                                    <button className="btn btn-warning btn-sm py-0 px-2 fw-semibold" onClick={() => handleAssign(ticket.id)}>В работу</button>
                                                )}
                                                {ticket.status === 2 && (
                                                    <button className="btn btn-success btn-sm py-0 px-2 fw-semibold" onClick={() => handleComplete(ticket.id)}>Выполнить</button>
                                                )}
                                                {/* Кнопка контекстного подробного меню */}
                                                <button 
                                                    className="btn btn-light btn-sm text-primary py-0 px-2 border fw-bold font-monospace" 
                                                    title="Детали заявки" 
                                                    onClick={() => setSelectedTicket(ticket)}
                                                >
                                                    •••
                                                </button>
                                            </div>
                                        ) : (
                                            <span className="small text-secondary">{ticket.technicianName || 'Не назначен'}</span>
                                        )}
                                    </td>
                                    
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
                {selectedTicket && (
                    <TicketDetailsModal 
                        ticket={selectedTicket} 
                        onClose={() => setSelectedTicket(null)} 
                        onAssign={handleAssign} 
                        onComplete={handleComplete} 
                        onCancel={handleCancel} 
                    />
                )}
        </div>
    );
}


export default TicketList;
