namespace Universityhelpdesk.Domain.Enums;

public enum EquipmentType : byte
{
    Computer = 1, // Компьютеры и моноблоки
    PrinterScanner, // Принтеры и сканеры
    ProjectorDisplay, // Проекторы, интерактивные доски, телевизоры
    NetworkDevice, // Роутеры, коммутаторы, отсутствие интернета
    AudioEquipment, // Колонки, микрофоны в лекционных аудиториях
    Other
}