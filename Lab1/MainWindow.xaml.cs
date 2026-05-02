using Lab1.Models;
using Lab1.Services;
using Lab1.Validation;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab1;

// Главное окно приложения «Менеджер привычек»
public partial class MainWindow : Window
{
    // Профиль текущего пользователя
    private readonly UserProfile profile = new();

    // Коллекция привычек, привязанная к DataGrid
    public ObservableCollection<Habit> Habits { get; } = HabitService.CreateSampleHabits();

    // Коллекция статистики по дням, привязанная к ListView
    private readonly ObservableCollection<DayStatistic> statistics = new();

    // Инициализирует компоненты окна и заполняет начальные данные
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        lvStatistics.ItemsSource = statistics;
        InitializeStatistics();
        UpdateStatus();
        statusDate.Text = DateTime.Now.ToShortDateString();
    }

    // ═══════════════════════════════════════════════════════════════════
    // ИНИЦИАЛИЗАЦИЯ
    // ═══════════════════════════════════════════════════════════════════

    // Заполняет коллекцию тестовой статистикой через StatisticsService
    private void InitializeStatistics()
    {
        foreach (var stat in StatisticsService.GenerateSampleStatistics(days: 7))
            statistics.Add(stat);
    }

    // ═══════════════════════════════════════════════════════════════════
    // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
    // ═══════════════════════════════════════════════════════════════════

    // Обновляет строку состояния с актуальным числом привычек и именем пользователя
    private void UpdateStatus()
    {
        int completed = Habits.Count(h => h.IsCompleted);
        statusText.Text = $"Готово | Привычек: {Habits.Count} (выполнено: {completed})";
        statusUser.Text = string.IsNullOrEmpty(profile.FullName)
            ? "Пользователь не задан"
            : profile.FullName;
    }

    // Пересчитывает прогресс дня как среднее продуктивности и удовлетворённости
    private void UpdateDayProgress()
    {
        if (pbDayProgress is null || slProductivity is null || slSatisfaction is null) return;
        pbDayProgress.Value = (slProductivity.Value + slSatisfaction.Value) / 2.0;
    }

    // ═══════════════════════════════════════════════════════════════════
    // ВКЛАДКА «ЛИЧНЫЕ ДАННЫЕ»
    // ═══════════════════════════════════════════════════════════════════

    // Сохраняет данные из формы в объект профиля после валидации
    private void SaveProfile_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateProfile()) return;

        profile.FirstName = txtFirstName.Text.Trim();
        profile.LastName = txtLastName.Text.Trim();
        profile.Password = pwdPassword.Password;
        profile.BirthDate = dtpBirthDate.SelectedDate;
        profile.Education = (cmbEducation.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
        profile.ReceiveNotifications = chkNotifications.IsChecked == true;
        profile.ShowPublicStats = chkPublicStats.IsChecked == true;
        profile.AutoSave = chkAutoSave.IsChecked == true;
        profile.ActivityLevel = rbHigh.IsChecked == true ? "Высокий"
                              : rbLow.IsChecked == true  ? "Низкий"
                              : "Средний";

        profile.Hobbies.Clear();
        foreach (ListBoxItem item in lstHobbies.SelectedItems)
            profile.Hobbies.Add(item.Content?.ToString() ?? "");

        lblValidation.Text = "✔ Данные сохранены успешно!";
        lblValidation.Foreground = Brushes.Green;
        SaveTodayStatistic();
        UpdateStatus();
    }

    // Проверяет корректность заполнения формы через ProfileValidator
    private bool ValidateProfile()
    {
        var result = ProfileValidator.ValidateAll(
            txtFirstName.Text,
            txtLastName.Text,
            dtpBirthDate.SelectedDate);

        if (!result.IsValid)
        {
            SetValidationError(result.ErrorMessage);
            return false;
        }
        lblValidation.Text = string.Empty;
        return true;
    }

    // Выводит сообщение об ошибке валидации красным цветом
    private void SetValidationError(string message)
    {
        lblValidation.Text = $"⚠ {message}";
        lblValidation.Foreground = Brushes.Red;
    }

    // Сбрасывает все поля формы личных данных к значениям по умолчанию
    private void ResetProfile_Click(object sender, RoutedEventArgs e)
    {
        txtFirstName.Text = string.Empty;
        txtLastName.Text = string.Empty;
        pwdPassword.Password = string.Empty;
        dtpBirthDate.SelectedDate = null;
        cmbEducation.SelectedIndex = -1;
        lstHobbies.SelectedItems.Clear();
        chkNotifications.IsChecked = false;
        chkPublicStats.IsChecked = false;
        chkAutoSave.IsChecked = true;
        chkDarkMode.IsChecked = false;
        rbMedium.IsChecked = true;
        lblValidation.Text = string.Empty;
    }

    // Открывает диалог выбора файла изображения и устанавливает аватар
    private void LoadAvatar_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
            Title = "Выберите изображение для аватара"
        };
        if (dlg.ShowDialog() != true) return;

        var bitmap = new BitmapImage(new Uri(dlg.FileName));
        imgAvatar.Source = bitmap;
        profile.AvatarPath = dlg.FileName;
    }

    // ═══════════════════════════════════════════════════════════════════
    // ВКЛАДКА «ПРИВЫЧКИ»
    // ═══════════════════════════════════════════════════════════════════

    // Добавляет новую привычку в коллекцию по данным из полей ввода
    private void AddHabit_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtHabitName.Text)) return;

        Habits.Add(new Habit
        {
            Name = txtHabitName.Text.Trim(),
            Time = txtHabitTime.Text.Trim(),
            IsCompleted = false
        });

        txtHabitName.Text = string.Empty;
        txtHabitTime.Text = string.Empty;
        UpdateStatus();
        UpdateDayProgress();
    }

    // Обновляет метку и ProgressBar при изменении значения слайдера продуктивности
    private void SlProductivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (lblProductivity is null) return;
        lblProductivity.Text = $"{(int)slProductivity.Value}%";
        UpdateDayProgress();
    }

    // Обновляет метку при изменении значения слайдера удовлетворённости
    private void SlSatisfaction_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (lblSatisfaction is null) return;
        lblSatisfaction.Text = $"{(int)slSatisfaction.Value}%";
        UpdateDayProgress();
    }

    // Обрабатывает выбор даты в Calendar и обновляет строку состояния
    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (calendar.SelectedDate.HasValue)
            statusDate.Text = calendar.SelectedDate.Value.ToShortDateString();
        UpdateStatus();
    }

    // Обрабатывает завершение редактирования ячейки DataGrid и обновляет статус
    private void DgHabits_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        Dispatcher.InvokeAsync(UpdateStatus);
    }

    // Сохраняет или обновляет запись статистики за сегодня в коллекции
    private void SaveTodayStatistic()
    {
        var todayStat = HabitService.BuildTodayStatistic(
            Habits, slProductivity.Value, slSatisfaction.Value);

        var existing = statistics.FirstOrDefault(s => s.Date == DateTime.Today);
        if (existing is not null)
            statistics.Remove(existing);

        statistics.Add(todayStat);
    }

    // ═══════════════════════════════════════════════════════════════════
    // ПАНЕЛЬ ИНСТРУМЕНТОВ
    // ═══════════════════════════════════════════════════════════════════

    // Увеличивает значение слайдера продуктивности на 1 при удержании RepeatButton
    private void RepeatUp_Click(object sender, RoutedEventArgs e)
    {
        if (slProductivity.Value < slProductivity.Maximum)
            slProductivity.Value++;
    }

    // Уменьшает значение слайдера продуктивности на 1 при удержании RepeatButton
    private void RepeatDown_Click(object sender, RoutedEventArgs e)
    {
        if (slProductivity.Value > slProductivity.Minimum)
            slProductivity.Value--;
    }

    // Показывает дополнительную боковую панель при активации ToggleButton
    private void ToggleEditMode_Checked(object sender, RoutedEventArgs e)
    {
        extraPanel.Visibility = Visibility.Visible;
        UpdateStatus();
    }

    // Скрывает дополнительную боковую панель при деактивации ToggleButton
    private void ToggleEditMode_Unchecked(object sender, RoutedEventArgs e)
    {
        extraPanel.Visibility = Visibility.Collapsed;
        UpdateStatus();
    }

    // ═══════════════════════════════════════════════════════════════════
    // МЕНЮ
    // ═══════════════════════════════════════════════════════════════════

    // Сохраняет профиль и показывает подтверждение
    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile_Click(sender, e);
        if (string.IsNullOrEmpty(lblValidation.Text) || lblValidation.Foreground == Brushes.Green)
            MessageBox.Show("Данные профиля сохранены!", "Сохранение",
                MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // Имитирует загрузку данных из файла (заглушка)
    private void MenuLoad_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Функция загрузки из файла будет реализована в следующей версии.",
            "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // Завершает работу приложения
    private void MenuExit_Click(object sender, RoutedEventArgs e) =>
        Application.Current.Shutdown();

    // Выполняет команду «Копировать» для активного элемента управления
    private void MenuCopy_Click(object sender, RoutedEventArgs e) =>
        ApplicationCommands.Copy.Execute(null, FocusManager.GetFocusedElement(this) as IInputElement);

    // Выполняет команду «Вставить» для активного элемента управления
    private void MenuPaste_Click(object sender, RoutedEventArgs e) =>
        ApplicationCommands.Paste.Execute(null, FocusManager.GetFocusedElement(this) as IInputElement);

    // Переключает интерфейс на тёмную тему через ResourceDictionary
    private void MenuDarkTheme_Click(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/DarkTheme.xaml");

    // Переключает интерфейс на светлую тему через ResourceDictionary
    private void MenuLightTheme_Click(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/LightTheme.xaml");

    // Обрабатывает включение чекбокса тёмного режима
    private void ChkDarkMode_Checked(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/DarkTheme.xaml");

    // Обрабатывает выключение чекбокса тёмного режима
    private void ChkDarkMode_Unchecked(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/LightTheme.xaml");

    // Применяет тему, заменяя словарь ресурсов темы в Application.Resources
    private static void ApplyTheme(string themeUri)
    {
        var dict = new ResourceDictionary
        {
            Source = new Uri(themeUri, UriKind.Relative)
        };

        // Находим старый словарь темы и удаляем его перед добавлением нового
        var existing = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") == true);

        if (existing is not null)
            Application.Current.Resources.MergedDictionaries.Remove(existing);

        Application.Current.Resources.MergedDictionaries.Add(dict);
    }

    // Обновляет строку состояния при переключении между вкладками
    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (statusText is null) return;
        string tab = (mainTabControl.SelectedItem as TabItem)?.Header?.ToString()?.Trim() ?? "";
        int completed = Habits.Count(h => h.IsCompleted);
        statusText.Text = $"Вкладка: «{tab}» | Привычек: {Habits.Count} (выполнено: {completed})";
    }
}
