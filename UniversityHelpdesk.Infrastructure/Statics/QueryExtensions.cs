using Universityhelpdesk.Application.DTOs;
using Universityhelpdesk.Domain.Entities;

namespace Universityhelpdesk.Infrastructure.Statics;

public static class QueryExtensions
{
    // Собираем запрос динамически, чтобы не засорять sql-запрос лишними проверками
    public static IQueryable<Ticket> WithFilters(this IQueryable<Ticket> query,  TicketFilterDTO filter)
    {
        if (filter.Status != null)
            query = query.Where(t => t.Status == filter.Status);

        if (filter.EquipmentType != null)
            query = query.Where(t => t.EquipmentType == filter.EquipmentType);

        if (filter.FromDate != null)
            query = query.Where(t => t.CreatedAt >= filter.FromDate);

        if (filter.ToDate != null)
            query = query.Where(t => t.CreatedAt <= filter.ToDate);

        query = filter.IsAscending 
            ? query.OrderBy(t => t.CreatedAt) 
            : query.OrderByDescending(t => t.CreatedAt);

        return query;
    }
}