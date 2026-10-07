const API_BASE_URL = "http://localhost:5133/api";

export const request = async (endpoint, options = {}) => {
    const token = localStorage.getItem("jwt_token");

    const headers = {
        "Content-Type": "application/json",
        ...options.headers,
    };

    if (token){
        headers["Authorization"] = `Bearer ${token}`;
    }

    const config = {
        ...options, 
        headers
    };

    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);

    if (response.status === 204) return null;

    const responseText = await response.text();

    let data;
    try {
        data = responseText ? JSON.parse(responseText.trim()) : {};
    } catch (e) {
        data = { message: responseText }
    }
    
    if (!response.ok){
        throw new Error(data.message || "Произошла ошибка при выполнении запроса");
    }

    return data;
};