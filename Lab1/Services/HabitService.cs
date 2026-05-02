using Lab1.Models;
using System.Collections.ObjectModel;

namespace Lab1.Services;

public static class HabitService
{
    public static ObservableCollection<Habit> CreateSampleHabits() =>
        new()
        {
            new Habit { Name = "Утренняя зарядка",           Time = "07:00", IsCompleted = true  },
            new Habit { Name = "Чтение книги",               Time = "20:00", IsCompleted = false },
            new Habit { Name = "Стакан воды",                Time = "08:00", IsCompleted = true  },
            new Habit { Name = "Прогулка на свежем воздухе", Time = "18:30", IsCompleted = false },
            new Habit { Name = "Медитация",                  Time = "21:00", IsCompleted = false },
        };

    public static double CalculateCompletionPercent(IEnumerable<Habit> habits)
    {
        var list = habits.ToList();
        if (list.Count == 0) return 0.0;
        return list.Count(h => h.IsCompleted) * 100.0 / list.Count;
    }

    public static DayStatistic BuildTodayStatistic(
        IEnumerable<Habit> habits, double productivity, double satisfaction)
    {
        var list = habits.ToList();
        return new DayStatistic
        {
            Date = DateTime.Today,
            Productivity = (int)productivity,
            Satisfaction = (int)satisfaction,
            CompletedHabits = list.Count(h => h.IsCompleted),
            Progress = (int)((productivity + satisfaction) / 2.0)
        };
    }
}
