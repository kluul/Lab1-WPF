namespace Lab1.Models;

// Статистика активности пользователя за один день
public class DayStatistic
{
    // Дата записи статистики
    public DateTime Date { get; set; }

    // Оценка продуктивности за день (0–100)
    public int Productivity { get; set; }

    // Оценка удовлетворённости за день (0–100)
    public int Satisfaction { get; set; }

    // Количество выполненных привычек за день
    public int CompletedHabits { get; set; }

    // Итоговый прогресс дня в процентах (0–100)
    public int Progress { get; set; }
}
