using Universityhelpdesk.Application.DTOs;
using Universityhelpdesk.Domain.Entities;

namespace Universityhelpdesk.Application.Interfaces;

public interface ITicketService
{
    Task<TicketDTO> CreateTicketAsync(CreateTicketDTO dto, int applicantId);
    Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(TicketFilterDTO filter);
    Task<TicketDTO?> GetTicketByIdAsync(int id);
    Task<IEnumerable<TicketDTO>> GetTicketsByRoleAsync(string role, int id, TicketFilterDTO filter);
    Task AssignTechnicianAsync(int ticketId, int technicianId);
    Task ChangeStatusAsync(int ticketId, int statusId);
    Task CompleteTicketAsync(int ticketId, int technicianId);
    Task CancelTicketAsync(int ticketId, int technicianId);
}