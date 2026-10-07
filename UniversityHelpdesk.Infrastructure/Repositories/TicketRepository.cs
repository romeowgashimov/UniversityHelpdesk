using Microsoft.EntityFrameworkCore;
using Universityhelpdesk.Application.DTOs;
using Universityhelpdesk.Application.Interfaces;
using Universityhelpdesk.Domain.Entities;
using Universityhelpdesk.Domain.Enums;
using Universityhelpdesk.Infrastructure.Data;
using Universityhelpdesk.Infrastructure.Statics;

namespace Universityhelpdesk.Infrastructure.Repositories;

public class TicketRepository(AppDbContext сontext) : ITicketRepository
{
    private readonly AppDbContext _context = сontext;

    private IQueryable<Ticket> Tickets => _context.Tickets
        .Include(t => t.Applicant)
        .Include(t => t.Technician);

    public async Task AddAsync(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync(TicketFilterDTO filter)
    {
        return await Tickets
            .AsNoTracking()
            .WithFilters(filter)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByApplicantIdAsync(int applicantId, TicketFilterDTO filter)
    {
        // Не отслеживаем изменения в сущностях, так как данные нужны только для чтения
        return await Tickets
            .AsNoTracking()
            .Where(t => t.ApplicantId == applicantId)
            .WithFilters(filter)
            .ToListAsync();
    }


    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await Tickets
            .AsNoTracking()
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByTechnicianIdAsync(int technicianId,  TicketFilterDTO filter)
    {
        return await Tickets
            .AsNoTracking()
            .Where(t => filter.ShowOnlyMine && t.TechnicianId == technicianId || t.Status == TicketStatus.New)
            .WithFilters(filter)
            .ToListAsync();
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }
}