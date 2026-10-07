using System.ComponentModel.DataAnnotations;
using Universityhelpdesk.Domain.Enums;

namespace Universityhelpdesk.Application.DTOs;

public class CreateTicketDTO
{
    [Required(ErrorMessage = "Необходимо выбрать тип оборудования")]
    [EnumDataType(typeof(EquipmentType), ErrorMessage = "Указан неверный тип оборудования")]
    public EquipmentType EquipmentType { get; set; }

    [Required(ErrorMessage = "Укажите детали оборудования (модель или инв. номер)")]
    [StringLength(100, ErrorMessage = "Название оборудования не должно превышать 100 символов")]
    public string EquipmentDetails { get; set; } = string.Empty;

    [Required(ErrorMessage = "Номер кабинета обязателен для заполнения")]
    //По-хорошему иметь номера аудиторий в базе данных
    [StringLength(10, ErrorMessage = "Номер кабинета не должен превышать 10 символов")]
    public string RoomNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пожалуйста, детально опишите проблему с оборудованием")]
    [StringLength(1000, ErrorMessage = "Описание проблемы не должно превышать 1000 символов")]
    public string Description { get; set; } = string.Empty;
}