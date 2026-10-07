using Universityhelpdesk.Domain.Enums;

namespace Universityhelpdesk.Application.DTOs;

public class TicketFilterDTO
{
    public TicketStatus? Status { get; set; }
    public EquipmentType? EquipmentType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool IsAscending { get; set; } = false;
    public bool ShowOnlyMine { get; set; } = false;
}