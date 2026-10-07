import { request } from "./api";

export const ticketService = {
    getMyTickets: async (filters = {}) => {
        const queryParams = new URLSearchParams();

        if (filters.status) queryParams.append("status", filters.status);
        if (filters.equipmentType) queryParams.append("equipmentType", filters.equipmentType);
        if (filters.fromDate) queryParams.append("fromDate", filters.fromDate);
        if (filters.toDate) queryParams.append("toDate", filters.toDate);
        if (filters.isAscending) queryParams.append("isAscending", filters.isAscending);
        if (filters.showOnlyMine !== undefined) {
            queryParams.append("showOnlyMine", filters.showOnlyMine);
        }
        
        const queryString = queryParams.toString();
        const url = queryString ? `/tickets/my?${queryString}` : `/tickets/my`;

        return await request(url, { method: "GET"});
    },

    createTicket: async (ticketData) => {
        return await request("/tickets", {
            method: "POST",
            body: JSON.stringify(ticketData)
        });
    },

    assign: async (ticketId) => {
        return await request(`/tickets/${ticketId}/assign`, { method: "PUT"});
    },

    complete: async (ticketId) => {
        return await request(`/tickets/${ticketId}/complete`, { method: "PUT"});
    },

    cancel: async (ticketId) => {
        return await request(`/tickets/${ticketId}/cancel`, { method: 'PUT' });
    }
};