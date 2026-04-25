namespace Lab1.Models;

/// <summary>Модель привычки пользователя для отображения в DataGrid.</summary>
public class Habit
{
    /// <summary>Название привычки.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Рекомендуемое время выполнения (например, "07:00").</summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>Признак выполнения привычки за выбранный день.</summary>
    public bool IsCompleted { get; set; }
}
