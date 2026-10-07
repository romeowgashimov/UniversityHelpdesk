import React, { useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { authService } from "../../services/authService"

function Login() {
    const { login } = useAuth();

    const [Login, setLogin] = useState("");
    const [Password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setLoading(false);

        if (!Login.trim() || !Password.trim()){
            setError("Пожалуйста, заполните все поля");
            return;
        }

        setLoading(true);
        try {
            const data = await authService.login(Login, Password);

            login(data);
        } catch (err){
            setError(err.message || "Произошла ошибка при входе")
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="row justify-content-center align-items-center" style={{ minHeight: "70vh"}}>
            <div className="col-md-6 col-lg-5">
                <div className="card shadow border-0 p-4">
                    <div className="card-body">
                        <div className="text-center mb-4">
                            <span className="fs-1">Сервис информационного и технического обслуживания</span>
                            <h2 className="fw-bold mt-2 text-primary">Вход в систему</h2>
                        </div>

                        {error && (
                            <div className="alert alert-danger d-flex align-items-center p-2 small" role="alert">
                                <div>Ошибка {error}</div>
                            </div>
                        )}

                        <form onSubmit={handleSubmit}>
                            <div className="mb-3">
                                <label className="form-label text-secondary small fw-semibold">Логин</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    placeholder="roman"
                                    value={Login}
                                    onChange={(e) => setLogin(e.target.value)}
                                    disabled={loading}
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label text-secondary small fw-semibold">Пароль</label>
                                <input
                                    type="password"
                                    className="form-control"
                                    placeholder="*******"
                                    value={Password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    disabled={loading}
                                />
                            </div>

                            <button
                                type="submit"
                                className="btn btn-primary w-100 fw-semibold mt-2"
                                disabled={loading}
                            >
                                {loading ? (
                                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                ) : "Войти"}
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Login;