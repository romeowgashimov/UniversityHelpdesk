import { request } from "./api";

export const authService = {
    login: async (login, password) => {
        return await request("/auth/login", {
            method: "POST",
            body: JSON.stringify({ login, password })
        });
    }
};