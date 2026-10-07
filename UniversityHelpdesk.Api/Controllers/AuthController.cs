using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Universityhelpdesk.Domain.Statics;
using Universityhelpdesk.Infrastructure.Data;
using UniversityHelpdesk.Application.DTOs;

namespace Universityhelpdesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AppDbContext context, 
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = await context
            .Users
            .FirstOrDefaultAsync(u => u.Login == dto.Login); 

        if (user == null)
            return Unauthorized($"Пользователь с таким логином не найден");

        if (!HashProvider.Equals(user.PasswordHash, dto.Password))
            return Unauthorized($"Неверный логин или пароль");

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var jwtSettings = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            role = user.Role.ToString(),
            fullName = user.FullName
        });
    }
}