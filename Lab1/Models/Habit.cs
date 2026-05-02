namespace Lab1.Models;

// Модель привычки пользователя для отображения в DataGrid
public class Habit
{
    // Название привычки
    public string Name { get; set; } = string.Empty;

    // Рекомендуемое время выполнения, например "07:00"
    public string Time { get; set; } = string.Empty;

    // Признак выполнения привычки за выбранный день
    public bool IsCompleted { get; set; }
}
