using Microsoft.Extensions.Logging;
using Universityhelpdesk.Application.DTOs;
using Universityhelpdesk.Application.Interfaces;
using Universityhelpdesk.Domain.Entities;
using Universityhelpdesk.Domain.Enums;

namespace Universityhelpdesk.Application.Services;

public class TicketService(ITicketRepository ticketRepository, ILogger<TicketService> logger) : ITicketService
{
    public async Task AssignTechnicianAsync(int ticketId, int technicianId)
    {
        logger.LogInformation("Техник Id: {TechId} пытается взять в работу заявку Id: {TicketId}", technicianId, ticketId);
        
        var ticket = await ticketRepository.GetByIdAsync(ticketId);
            
        if (ticket == null)
        {
            logger.LogWarning("Провалено назначение: Заявка Id: {TicketId} не найдена", ticketId);
            throw new KeyNotFoundException($"Заявка не найдена");   
        }

        if (ticket.Status != TicketStatus.New)
        {
            logger.LogWarning("Техник Id: {TechId} не смог взять заявку Id: {TicketId}, так как её текущий статус: {Status}", technicianId, ticketId, ticket.Status);
            throw new InvalidOperationException($"Назначить мастера можно только на новую заявку");
        }

        ticket.TechnicianId = technicianId;
        ticket.Status = TicketStatus.InProgress;

        await ticketRepository.UpdateAsync(ticket);
        logger.LogInformation("Заявка Id: {TicketId} успешно закреплена за техником Id: {TechId}", ticketId, technicianId);
    }

    public async Task ChangeStatusAsync(int ticketId, int statusId)
    {
        var ticket = await ticketRepository.GetByIdAsync(ticketId)
            ?? throw new KeyNotFoundException($"Заявка не найдена");

        if (!Enum.IsDefined(typeof(TicketStatus), statusId))
            throw new ArgumentException($"Указан неверный статус");

        ticket.Status = (TicketStatus)statusId;
        await ticketRepository.UpdateAsync(ticket);
    }

    public async Task<TicketDTO> CreateTicketAsync(CreateTicketDTO dto, int applicantId)
    {
        logger.LogInformation("Пользователь Id: {ApplicantId} инициализировал создание заявки для оборудования типа {Type}", applicantId, dto.EquipmentType);
        
        var ticket = new Ticket
        {
            EquipmentType = dto.EquipmentType,
            EquipmentDetails = dto.EquipmentDetails,
            RoomNumber = dto.RoomNumber,
            Description = dto.Description,
            ApplicantId = applicantId,
            Status = TicketStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        await ticketRepository.AddAsync(ticket);

        logger.LogInformation("Заявка успешно создана в БД. Присвоенный id: {TicketId}", ticket.Id);

        var savedTicket = await ticketRepository.GetByIdAsync(ticket.Id);
        return MapToDTO(savedTicket ?? ticket);
    }

    private static TicketDTO MapToDTO(Ticket ticket)
    {
        return new TicketDTO
        {
            Id = ticket.Id,
            EquipmentType = ticket.EquipmentType,
            EquipmentDetails = ticket.EquipmentDetails,
            RoomNumber = ticket.RoomNumber,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            ApplicantName = ticket.Applicant?.FullName ?? $"Id: {ticket.ApplicantId}",
            TechnicianName = ticket.Technician?.FullName
        };
    }

    public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(TicketFilterDTO filter)
    {
        var tickets = await ticketRepository.GetAllAsync(filter);
        return tickets.Select(MapToDTO);
    }

    public async Task<TicketDTO?> GetTicketByIdAsync(int id)
    {
        var ticket = await ticketRepository.GetByIdAsync(id);
        return ticket == null ? null : MapToDTO(ticket);
    }

    public async Task CompleteTicketAsync(int ticketId, int technicianId)
    {
        var ticket = await ticketRepository.GetByIdAsync(ticketId);
            
        if (ticket == null) 
        {
            logger.LogWarning("Провалено закрытие: заявка Id {TicketId} не найдена", ticketId);
            throw new KeyNotFoundException($"Заявка с Id {ticketId} не найдена");
        }

        if (ticket.TechnicianId == null)
        {
            logger.LogWarning("Провалено закрытие: заявка Id: {TicketId}: мастер не был назначен", ticketId);
            throw new InvalidOperationException($"Нельзя завершить заявку, на которую не назначен техник");
        }
        
        if (ticket.TechnicianId != technicianId)
        {
            logger.LogWarning("Отказ в доступе: Техник Id: {TechId} пытался закрыть заявку Id: {TicketId} (Исполнитель заявки: Id: {ActualTechId})",technicianId, ticketId, ticket.TechnicianId);
            throw new UnauthorizedAccessException($"Вы не можете завершить заявку, на которую назначен другой сотрудник");
        }

        if (ticket.Status == TicketStatus.Resolved || ticket.Status == TicketStatus.Cancelled)
        {
            logger.LogWarning("Провалено закрытие заявки Id: {TicketId}: она уже находится в статусе {Status}", ticketId, ticket.Status);
            throw new InvalidOperationException($"Заявка уже закрыта или отменена");
        }

        ticket.Status = TicketStatus.Resolved;
        await ticketRepository.UpdateAsync(ticket);
        logger.LogInformation("Заявка Id: {TicketId} успешно выполнена техником Id: {TechId}", ticketId, technicianId);
    }

    // Сделать доступ только по идентификатору, роль искать в самой базе данных
    public async Task<IEnumerable<TicketDTO>> GetTicketsByRoleAsync(string role, int id, TicketFilterDTO filter)
    {
        IEnumerable<Ticket> tickets;
        switch (role)
        {
            case nameof(UserRole.Applicant):
                tickets = await ticketRepository.GetByApplicantIdAsync(id, filter);
                return tickets.Select(MapToDTO);

            case nameof(UserRole.Technician):
                tickets = await ticketRepository.GetByTechnicianIdAsync(id, filter);
                return tickets.Select(MapToDTO);

            default:
                throw new KeyNotFoundException($"Найти заявки у пользователя с идентификатором: {id} и ролью: {role} не удалось");
        }
    }

    public async Task CancelTicketAsync(int ticketId, int technicianId)
    {
        var ticket = await ticketRepository.GetByIdAsync(ticketId) 
            ?? throw new KeyNotFoundException($"Заявка {ticketId} не найдена");

        if (ticket.TechnicianId != technicianId)
        {
            logger.LogWarning("Отказ в доступе: Техник Id: {TechId} пытался отменить заявку Id: {TicketId} (Исполнитель заявки: Id: {ActualTechId})",technicianId, ticketId, ticket.TechnicianId);
            throw new UnauthorizedAccessException("ВЫ не можете отменить чужую заявку");
        }

        if (ticket.Status == TicketStatus.Resolved || ticket.Status == TicketStatus.Cancelled)
        {
            logger.LogWarning("Провалена отмена заявки Id: {TicketId}: она уже находится в статусе {Status}", ticketId, ticket.Status);
            throw new InvalidOperationException("Заявка уже закрыта или отменена");
        }

        ticket.Status = TicketStatus.Cancelled;
        await ticketRepository.UpdateAsync(ticket);
        logger.LogInformation("Заявка ID:{TicketId} успешно ОТМЕНЕНА техником ID:{TechId}.", ticketId, technicianId);
    }
}