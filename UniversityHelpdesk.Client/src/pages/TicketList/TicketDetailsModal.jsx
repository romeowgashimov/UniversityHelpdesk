import React, { useState } from "react";

function TicketDetailsModal({ ticket, onClose, onAssign, onComplete, onCancel }){
    const [actionLoading, setActionLoading] = useState(false);
    const [actionError, setActionError] = useState('');

    if(!ticket) return null;

    const handleAction = async (actionFn) => {
        setActionError('');
        setActionLoading(true);
        try {
            await actionFn(ticket.id);
            onClose();
        } catch (err){
            setActionError(err.message || 'Не удалось выполнить действие');
        } finally {
            setActionLoading(false);
        }
    };

    const getEquipmentTypeName = (type) => {
        const types = {
            1: 'Компьютер',
            2: 'Принтер/Сканер',
            3: 'Проектор/Экран',
            4: 'Сетевое устройство',
            5: 'Аудио',
            6: 'Другое'
        };
        return types[type] || 'Неизвестно';
    };

    return (
        <div className="modal d-block fade show" tabIndex="-1" style={{ background: "rgba(0,0,0,0.5)" }}>
            <div className="modal-dialog modal-dialog-centered">
                <div className="modal-content border-0 shadow-lg bg-white">
                    <div className="modal-header bg-dark text-white">
                        <h5 className="modal-title fw-bold">Управление заявкой #{ticket.id}</h5>
                        <button type="button" className="btn-close btn-close-white" onClick={onClose} disabled={actionLoading}></button>
                    </div>
                    <div className="modal-body p-4">
                        {actionError && <div className="alert alert-danger small p-2">Ошибка {actionError}</div>}

                        <div className="mb-3 border-bottom pb-2">
                            <span className="text-secondary small d-block fw-semibold">Кабинет & Оборудование</span>
                            <strong>{ticket.roomNumber}</strong> - {getEquipmentTypeName(ticket.equipmentType)} ({ticket.equipmentDetails})
                        </div>

                        <div className="mb-3 border-bottom pb-2">
                            <span className="text-secondary small d-block fw-semibold">Описание поломки</span>
                            <p className="small mb-1 text-wrap text-break">{ticket.description}</p>
                        </div>

                        <div className="mb-3 border-bottom pb-2">
                            <span className="text-secondary small d-block fw-semibold">Заявитель</span>
                            <span className="small text-dark">{ticket.applicantName}</span>
                        </div>

                        <div className="bg-light p-3 rounded mb-3 border">
                            <span className="text-muted small d-block fw-semibold mb-1">Отчёт о ремонте</span>
                            <textarea
                                className="form-control form-control-sm"
                                rows="2"
                                placeholder="Будущий функционал"
                                disabled>
                            </textarea>
                        </div>
                    </div>

                    <div className="modal-footer border-0 d-flex justify-content-between pt-0">
                        <button type="button" className="btn btn-sm btn-secondary fw-semibold" onClick={onClose} disabled={actionLoading}>
                            Закрыть
                        </button>

                        <div className="d-flex g-2">
                            {ticket.status === 1 && (
                                <button type="button" className="btn btn-sm btn-warning fw-semibold ms-2" onClick={() => handleAction(onAssign)} disabled={actionLoading}>
                                    В работу
                                </button>
                            )}

                            {ticket.status === 2 && (
                                <>
                                    <button type="button" className="btn btn-sm btn-outline-danger fw-semibold ms-2" onClick={() => handleAction(onCancel)} disabled={actionLoading}>
                                        Отменить
                                    </button>

                                    <button type="button" className="btn btn-sm btn-success fw-semibold ms-2" onClick={() => handleAction(onComplete)} disabled={actionLoading}>
                                        Выполнить
                                    </button>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default TicketDetailsModal;