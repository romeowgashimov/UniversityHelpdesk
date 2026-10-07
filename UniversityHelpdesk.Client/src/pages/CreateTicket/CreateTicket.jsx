import React, { useState } from "react";
import { ticketService } from "../../services/ticketService";

function CreateTicket({ onTicketCreated }){
    const [showModal, setShowModal] = useState(false);

    const [formData, setFormData] = useState({
        equipmentType: "1",
        equipmentDetails: "",
        roomNumber: "",
        description: ""
    });

    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({...prev, [name]: value }));
    };

    const handleClose = () => {
        setShowModal(false);
        setError("");
        setSuccess(false);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccess(false);

        const descTrim = formData.description.trim();

        if (!formData.equipmentDetails.trim() 
            || !formData.roomNumber.trim() 
            || !descTrim) {
            setError("Пожалуйста, заполните все обязательные поля");
            return;
        }

        if (descTrim.length < 10) {
            setError("Описание проблемы должно содержать не менее 10 символов")
            return;
        }

        setLoading(true);
        try {
            const payload = {
                ...formData, 
                equipmentType: parseInt(formData.equipmentType)
            };
            await ticketService.createTicket(payload);
            setSuccess(true);
            setFormData({
                equipmentType: "1",
                equipmentDetails: "",
                roomNumber: "",
                description: ""
            });
            if (onTicketCreated) onTicketCreated();

            setTimeout(() => {
                handleClose();
            }, 1000);

        } catch (err) {
            setError(err.message || "Не уадлось отправить заявку");
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <div className="text-start mb-3">
                <button className="btn btn-primary fw-semibold shadow-sm" onClick={() => setShowModal(true)}>
                    Создать новую заявку
                </button>
            </div>


            {showModal && (
                <>
                    <div className="modal d-block fade show" tabIndex="-1" style={{ backgroundColor: "rgba(0,0,0,0.5)" }}>
                        <div className="modal-dialog modal-dialog-centered">
                            <div className="modal-content border-0 shadow-lg">
                                <div className="modal-header bg-primary text-white">
                                    <h5 className="modal-title fw-bold">Новая заявка на ремонт</h5>
                                    <button type="button" className="btn-close btn-close-white" onClick={handleClose} aria-label="Close"></button>
                                </div>
                                <div className="modal-body p-4">
                                    {error && <div className="alert alert-danger small p-2">{error}</div>}
                                    {success && <div className="alert alert-success small p-2">Заявка успешно отправлена</div>}

                                    <form onSubmit={handleSubmit}>
                                        <div className="row g-3">
                                            <div className="col-md-6">
                                                <label className="form-label small fw-semibold text-muted">Тип оборудования</label>
                                                <select className="form-select form-select-sm" name="equipmentType" value={formData.equipmentType} onChange={handleChange} disabled={loading}>
                                                    <option value="1">Компьютер</option>
                                                    <option value="2">Принтер/Сканер</option>
                                                    <option value="3">Проектор/Экран</option>
                                                    <option value="4">Сетевое оборудование</option>
                                                    <option value="5">Аудиооборудование</option>
                                                    <option value="6">Другое</option>
                                                </select>
                                            </div>

                                            <div className="col-md-6">
                                                <label className="form-label small fw-semibold text-muted">Номер аудитории / Кабинет</label>
                                                <input type="text" className="form-control form-control-sm" name="roomNumber" placeholder="Ауд. 666" value={formData.roomNumber} onChange={handleChange} disabled={loading} required></input>
                                            </div>

                                            <div className="col-12">
                                                <label className="form-label small fw-semibold text-muted">Сведения о технике</label>
                                                <input type="text" className="form-control form-control-sm" name="equipmentDetails" placeholder="Модель, инвентарный номер" value={formData.equipmentDetails} onChange={handleChange} disabled={loading} required></input>
                                            </div>

                                            <div className="col-12">
                                                <label className="form-label small fw-semibold text-muted">Описание неисправности (детально)</label>
                                                <textarea className="form-control form-control-sm" name="description" rows="3" placeholder="Опишите, что именно произошло (не включается, замял бумагу, нет сигнала и т.д." value={formData.description} onChange={handleChange} disabled={loading} required></textarea>
                                            </div>

                                            <div className="modal-footer border-0 col-12 d-flex justify-content-end pb-0 pe-0">
                                                <button type="button" className="btn btn-sm btn-secondary me-2 fw-semibold" onClick={handleClose} disabled={loading}>
                                                    Отмена
                                                </button>
                                                <button type="submit" className="btn btn-sm btn-primary fw-semibold px-3" disabled={loading}>
                                                    {loading ? <span className="spinner-border spinner-border-sm me-2" role="status"></span> : "Отправить заявку"}
                                                </button>
                                            </div>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            )}
        </>
    );
}

export default CreateTicket;