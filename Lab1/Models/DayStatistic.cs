namespace Lab1.Models;

/// <summary>Статистика активности пользователя за один день.</summary>
public class DayStatistic
{
    /// <summary>Дата записи статистики.</summary>
    public DateTime Date { get; set; }

    /// <summary>Оценка продуктивности за день (0–100).</summary>
    public int Productivity { get; set; }

    /// <summary>Оценка удовлетворённости за день (0–100).</summary>
    public int Satisfaction { get; set; }

    /// <summary>Количество выполненных привычек за день.</summary>
    public int CompletedHabits { get; set; }

    /// <summary>Итоговый прогресс дня в процентах (0–100).</summary>
    public int Progress { get; set; }
}
