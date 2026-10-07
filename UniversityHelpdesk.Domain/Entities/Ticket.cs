using Universityhelpdesk.Domain.Enums;

namespace Universityhelpdesk.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }

    public EquipmentType EquipmentType { get; set; }
    // ???
    public string EquipmentDetails { get; set; } = string.Empty;

    // Кабинет или аудитория, в которой есть проблема
    public string RoomNumber { get; set; } = string.Empty;

    // Описание проблемы
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TicketStatus Status { get; set; } = TicketStatus.New;

    // Внешний ключ отправителя заявки
    public int ApplicantId { get; set; }
    public User? Applicant { get; set; }

    // При создании заявки и перед реагированием на неё 
    // внешний ключ сотрудника, обрабатывающего заявку, пустой
    public int? TechnicianId { get; set; }
    public User? Technician { get; set; }
}