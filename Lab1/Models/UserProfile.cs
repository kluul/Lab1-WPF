namespace Lab1.Models;

/// <summary>Профиль пользователя, хранящий всю персональную информацию.</summary>
public class UserProfile
{
    /// <summary>Имя пользователя.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Фамилия пользователя.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Пароль пользователя (имитация хранения).</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Дата рождения пользователя.</summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>Уровень образования.</summary>
    public string Education { get; set; } = string.Empty;

    /// <summary>Список выбранных хобби.</summary>
    public List<string> Hobbies { get; set; } = new();

    /// <summary>Флаг: получать уведомления.</summary>
    public bool ReceiveNotifications { get; set; }

    /// <summary>Флаг: показывать статистику публично.</summary>
    public bool ShowPublicStats { get; set; }

    /// <summary>Флаг: автосохранение данных.</summary>
    public bool AutoSave { get; set; } = true;

    /// <summary>Уровень физической активности: Низкий, Средний или Высокий.</summary>
    public string ActivityLevel { get; set; } = "Средний";

    /// <summary>Путь к файлу аватара пользователя.</summary>
    public string AvatarPath { get; set; } = string.Empty;

    /// <summary>Возвращает полное имя пользователя (Имя Фамилия).</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
