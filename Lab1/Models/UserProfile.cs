namespace Lab1.Models;

// Профиль пользователя, хранящий всю персональную информацию
public class UserProfile
{
    // Имя пользователя
    public string FirstName { get; set; } = string.Empty;

    // Фамилия пользователя
    public string LastName { get; set; } = string.Empty;

    // Пароль пользователя (имитация хранения)
    public string Password { get; set; } = string.Empty;

    // Дата рождения пользователя
    public DateTime? BirthDate { get; set; }

    // Уровень образования
    public string Education { get; set; } = string.Empty;

    // Список выбранных хобби
    public List<string> Hobbies { get; set; } = new();

    // Флаг: получать уведомления
    public bool ReceiveNotifications { get; set; }

    // Флаг: показывать статистику публично
    public bool ShowPublicStats { get; set; }

    // Флаг: автосохранение данных
    public bool AutoSave { get; set; } = true;

    // Уровень физической активности: Низкий, Средний или Высокий
    public string ActivityLevel { get; set; } = "Средний";

    // Путь к файлу аватара пользователя
    public string AvatarPath { get; set; } = string.Empty;

    // Возвращает полное имя пользователя (Имя Фамилия)
    public string FullName => $"{FirstName} {LastName}".Trim();
}
