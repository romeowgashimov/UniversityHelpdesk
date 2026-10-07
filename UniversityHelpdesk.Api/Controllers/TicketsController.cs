using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universityhelpdesk.Application.DTOs;
using Universityhelpdesk.Application.Interfaces;

namespace Universityhelpdesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAll([FromQuery] TicketFilterDTO filter)
    {
        var tickets = await ticketService.GetAllTicketsAsync(filter);
        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDTO>> GetById(int id)
    {
        var ticket = await ticketService.GetTicketByIdAsync(id);
        return ticket == null 
            ? NotFound($"Заявка с Id {id} не найдена") 
            : Ok(ticket);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TicketDTO>> Create([FromBody] CreateTicketDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized($"Не удалось определить пользователя");

        int currentUserId = int.Parse(userIdClaim.Value);

        var createdTicket = await ticketService.CreateTicketAsync(dto, currentUserId);

        return CreatedAtAction(nameof(GetById), 
            new { id = createdTicket.Id }, createdTicket);
    }

    [Authorize(Roles = "Technician")]
    [HttpPut("{id:int}/assign")]
    public async Task<IActionResult> AssignTechnician(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int technicianId = int.Parse(userIdClaim.Value);

        try
        {
            await ticketService.AssignTechnicianAsync(id, technicianId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Technician")]
    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> CompleteTicket(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int technicianId = int.Parse(userIdClaim.Value);

        try
        {
            await ticketService.CompleteTicketAsync(id, technicianId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TicketDTO>>> GetMyTickets([FromQuery] TicketFilterDTO filter)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userRoleClaim = User.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null || userRoleClaim == null)
            return Unauthorized();

        int currentUserId = int.Parse(userIdClaim.Value);
        string currentRole = userRoleClaim.Value;

        try
        {
            var tickets = await ticketService.GetTicketsByRoleAsync(currentRole, currentUserId, filter);
            return Ok(tickets);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> CancelTicket(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        int currentTechId = int.Parse(userIdClaim.Value);

        try
        {
            await ticketService.CancelTicketAsync(id, currentTechId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}