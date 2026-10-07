using Universityhelpdesk.Domain.Enums;

namespace Universityhelpdesk.Application.DTOs;

public class TicketDTO
{
    public int Id { get; set; }
    public EquipmentType EquipmentType { get; set; }
    public string EquipmentDetails { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string? TechnicianName { get; set; }
}