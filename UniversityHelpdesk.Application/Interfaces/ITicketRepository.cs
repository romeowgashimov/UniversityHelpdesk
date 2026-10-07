using Universityhelpdesk.Application.DTOs;
using Universityhelpdesk.Domain.Entities;

namespace Universityhelpdesk.Application.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(int id);
    Task<IEnumerable<Ticket>> GetAllAsync(TicketFilterDTO filter);
    Task<IEnumerable<Ticket>> GetByApplicantIdAsync(int applicantId, TicketFilterDTO filter);
    Task<IEnumerable<Ticket>> GetByTechnicianIdAsync(int technicianId, TicketFilterDTO filter);
    Task AddAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
}