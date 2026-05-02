using Lab1.Models;

namespace Lab1.Services;

public static class StatisticsService
{
    public static double AverageProductivity(IEnumerable<DayStatistic> stats)
    {
        var list = stats.ToList();
        return list.Count == 0 ? 0 : list.Average(s => s.Productivity);
    }

    public static double AverageSatisfaction(IEnumerable<DayStatistic> stats)
    {
        var list = stats.ToList();
        return list.Count == 0 ? 0 : list.Average(s => s.Satisfaction);
    }

    public static DayStatistic? BestDay(IEnumerable<DayStatistic> stats) =>
        stats.OrderByDescending(s => s.Progress).FirstOrDefault();

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
