using Lab1.Models;

namespace Lab1.Services;

/// <summary>Сервис расчёта и агрегации статистики привычек пользователя.</summary>
public static class StatisticsService
{
    /// <summary>Вычисляет среднюю продуктивность по заданной коллекции статистики.</summary>
    /// <param name="stats">Коллекция записей статистики.</param>
    /// <returns>Среднее значение продуктивности (0–100) или 0 если коллекция пустая.</returns>
    public static double AverageProductivity(IEnumerable<DayStatistic> stats)
    {
        var list = stats.ToList();
        return list.Count == 0 ? 0 : list.Average(s => s.Productivity);
    }

    /// <summary>Вычисляет среднюю удовлетворённость по заданной коллекции статистики.</summary>
    /// <param name="stats">Коллекция записей статистики.</param>
    /// <returns>Среднее значение удовлетворённости (0–100) или 0 если коллекция пустая.</returns>
    public static double AverageSatisfaction(IEnumerable<DayStatistic> stats)
    {
        var list = stats.ToList();
        return list.Count == 0 ? 0 : list.Average(s => s.Satisfaction);
    }

    /// <summary>Находит день с наибольшим прогрессом в коллекции.</summary>
    /// <param name="stats">Коллекция записей статистики.</param>
    /// <returns>Запись с максимальным прогрессом или null если коллекция пустая.</returns>
    public static DayStatistic? BestDay(IEnumerable<DayStatistic> stats) =>
        stats.OrderByDescending(s => s.Progress).FirstOrDefault();

    /// <summary>
    /// Генерирует тестовую статистику за указанное количество последних дней.
    /// </summary>
    /// <param name="days">Число дней для генерации.</param>
    /// <param name="seed">Зерно генератора случайных чисел для воспроизводимости.</param>
    /// <returns>Список записей статистики от самой старой к самой новой.</returns>
    public static List<DayStatistic> GenerateSampleStatistics(int days = 7, int seed = 42)
    {
        var rng = new Random(seed);
        var result = new List<DayStatistic>(days);

        for (int i = days - 1; i >= 0; i--)
        {
            int productivity = rng.Next(40, 96);
            int satisfaction = rng.Next(35, 91);
            result.Add(new DayStatistic
            {
                Date = DateTime.Today.AddDays(-i),
                Productivity = productivity,
                Satisfaction = satisfaction,
                CompletedHabits = rng.Next(1, 7),
                Progress = (productivity + satisfaction) / 2
            });
        }

        return result;
    }
}
