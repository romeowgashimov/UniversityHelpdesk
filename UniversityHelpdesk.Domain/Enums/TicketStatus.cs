namespace Universityhelpdesk.Domain.Enums;

public enum TicketStatus : byte
{
    New = 1, // Новая заявка
    InProgress, // На обслуживании
    Resolved, // Решена
    Cancelled, // Отменена

    Error = byte.MaxValue // Непредвиденная ошибка
}