using System.ComponentModel.DataAnnotations;

namespace UniversityHelpdesk.Application.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "Логин обязателен для входа")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен для входа")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}