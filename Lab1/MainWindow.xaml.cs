using Lab1.Models;
using Lab1.Validation;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab1;

/// <summary>Главное окно приложения «Менеджер привычек».</summary>
public partial class MainWindow : Window
{
    /// <summary>Профиль текущего пользователя.</summary>
    private readonly UserProfile _profile = new();

    /// <summary>Коллекция привычек, привязанная к DataGrid.</summary>
    public ObservableCollection<Habit> Habits { get; } = new();

    /// <summary>Коллекция статистики по дням, привязанная к ListView.</summary>
    private readonly ObservableCollection<DayStatistic> _statistics = new();

    /// <summary>Инициализирует компоненты окна и заполняет начальные данные.</summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        lvStatistics.ItemsSource = _statistics;
        InitializeStatistics();
        UpdateStatus();
        statusDate.Text = DateTime.Now.ToShortDateString();
    }

    // ═══════════════════════════════════════════════════════════════════
    // ИНИЦИАЛИЗАЦИЯ
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Заполняет коллекцию тестовой статистикой за последние 7 дней.</summary>
    private void InitializeStatistics()
    {
        var rng = new Random(42);
        for (int i = 6; i >= 0; i--)
        {
            _statistics.Add(new DayStatistic
            {
                Date = DateTime.Today.AddDays(-i),
                Productivity = rng.Next(40, 95),
                Satisfaction = rng.Next(35, 90),
                CompletedHabits = rng.Next(1, 6),
                Progress = rng.Next(30, 100)
            });
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Обновляет строку состояния с актуальным числом привычек и именем пользователя.</summary>
    private void UpdateStatus()
    {
        int completed = Habits.Count(h => h.IsCompleted);
        statusText.Text = $"Готово | Привычек: {Habits.Count} (выполнено: {completed})";
        statusUser.Text = string.IsNullOrEmpty(_profile.FullName)
            ? "Пользователь не задан"
            : _profile.FullName;
    }

    /// <summary>Пересчитывает прогресс дня как среднее продуктивности и удовлетворённости.</summary>
    private void UpdateDayProgress()
    {
        if (pbDayProgress is null || slProductivity is null || slSatisfaction is null) return;
        pbDayProgress.Value = (slProductivity.Value + slSatisfaction.Value) / 2.0;
    }

    // ═══════════════════════════════════════════════════════════════════
    // ВКЛАДКА «ЛИЧНЫЕ ДАННЫЕ»
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Сохраняет данные из формы в объект профиля после валидации.</summary>
    private void SaveProfile_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateProfile()) return;

        _profile.FirstName = txtFirstName.Text.Trim();
        _profile.LastName = txtLastName.Text.Trim();
        _profile.Password = pwdPassword.Password;
        _profile.BirthDate = dtpBirthDate.SelectedDate;
        _profile.Education = (cmbEducation.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
        _profile.ReceiveNotifications = chkNotifications.IsChecked == true;
        _profile.ShowPublicStats = chkPublicStats.IsChecked == true;
        _profile.AutoSave = chkAutoSave.IsChecked == true;
        _profile.ActivityLevel = rbHigh.IsChecked == true ? "Высокий"
                                : rbLow.IsChecked == true ? "Низкий"
                                : "Средний";

        _profile.Hobbies.Clear();
        foreach (ListBoxItem item in lstHobbies.SelectedItems)
            _profile.Hobbies.Add(item.Content?.ToString() ?? "");

        lblValidation.Text = "✔ Данные сохранены успешно!";
        lblValidation.Foreground = Brushes.Green;
        UpdateStatus();
    }

    /// <summary>Проверяет корректность заполнения формы через ProfileValidator.</summary>
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

    /// <summary>Выводит сообщение об ошибке валидации красным цветом.</summary>
    /// <param name="message">Текст ошибки для отображения.</param>
    private void SetValidationError(string message)
    {
        lblValidation.Text = $"⚠ {message}";
        lblValidation.Foreground = Brushes.Red;
    }

    /// <summary>Сбрасывает все поля формы личных данных к значениям по умолчанию.</summary>
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

    /// <summary>Открывает диалог выбора файла изображения и устанавливает аватар.</summary>
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
        _profile.AvatarPath = dlg.FileName;
    }

    // ═══════════════════════════════════════════════════════════════════
    // ВКЛАДКА «ПРИВЫЧКИ»
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Добавляет новую привычку в коллекцию по данным из полей ввода.</summary>
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

    /// <summary>Обновляет метку и ProgressBar при изменении значения слайдера продуктивности.</summary>
    private void SlProductivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (lblProductivity is null) return;
        lblProductivity.Text = $"{(int)slProductivity.Value}%";
        UpdateDayProgress();
    }

    /// <summary>Обновляет метку при изменении значения слайдера удовлетворённости.</summary>
    private void SlSatisfaction_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (lblSatisfaction is null) return;
        lblSatisfaction.Text = $"{(int)slSatisfaction.Value}%";
        UpdateDayProgress();
    }

    /// <summary>Обрабатывает выбор даты в Calendar и обновляет строку состояния.</summary>
    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (calendar.SelectedDate.HasValue)
            statusDate.Text = calendar.SelectedDate.Value.ToShortDateString();
        UpdateStatus();
    }

    /// <summary>Обрабатывает завершение редактирования ячейки DataGrid и обновляет статус.</summary>
    private void DgHabits_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        Dispatcher.InvokeAsync(UpdateStatus);
    }

    // ═══════════════════════════════════════════════════════════════════
    // ПАНЕЛЬ ИНСТРУМЕНТОВ
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Увеличивает значение слайдера продуктивности на 1 при удержании RepeatButton.</summary>
    private void RepeatUp_Click(object sender, RoutedEventArgs e)
    {
        if (slProductivity.Value < slProductivity.Maximum)
            slProductivity.Value++;
    }

    /// <summary>Уменьшает значение слайдера продуктивности на 1 при удержании RepeatButton.</summary>
    private void RepeatDown_Click(object sender, RoutedEventArgs e)
    {
        if (slProductivity.Value > slProductivity.Minimum)
            slProductivity.Value--;
    }

    /// <summary>Показывает дополнительную боковую панель при активации ToggleButton.</summary>
    private void ToggleEditMode_Checked(object sender, RoutedEventArgs e)
    {
        extraPanel.Visibility = Visibility.Visible;
        UpdateStatus();
    }

    /// <summary>Скрывает дополнительную боковую панель при деактивации ToggleButton.</summary>
    private void ToggleEditMode_Unchecked(object sender, RoutedEventArgs e)
    {
        extraPanel.Visibility = Visibility.Collapsed;
        UpdateStatus();
    }

    // ═══════════════════════════════════════════════════════════════════
    // МЕНЮ
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Сохраняет профиль и показывает подтверждение.</summary>
    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile_Click(sender, e);
        if (string.IsNullOrEmpty(lblValidation.Text) || lblValidation.Foreground == Brushes.Green)
            MessageBox.Show("Данные профиля сохранены!", "Сохранение",
                MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>Имитирует загрузку данных из файла (заглушка).</summary>
    private void MenuLoad_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Функция загрузки из файла будет реализована в следующей версии.",
            "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>Завершает работу приложения.</summary>
    private void MenuExit_Click(object sender, RoutedEventArgs e) =>
        Application.Current.Shutdown();

    /// <summary>Выполняет команду «Копировать» для активного элемента управления.</summary>
    private void MenuCopy_Click(object sender, RoutedEventArgs e) =>
        ApplicationCommands.Copy.Execute(null, FocusManager.GetFocusedElement(this) as IInputElement);

    /// <summary>Выполняет команду «Вставить» для активного элемента управления.</summary>
    private void MenuPaste_Click(object sender, RoutedEventArgs e) =>
        ApplicationCommands.Paste.Execute(null, FocusManager.GetFocusedElement(this) as IInputElement);

    /// <summary>Переключает интерфейс на тёмную тему (тёмный фон окна).</summary>
    private void MenuDarkTheme_Click(object sender, RoutedEventArgs e)
    {
        Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
    }

    /// <summary>Переключает интерфейс на светлую тему (белый фон окна).</summary>
    private void MenuLightTheme_Click(object sender, RoutedEventArgs e)
    {
        Background = new SolidColorBrush(Colors.White);
    }

    /// <summary>Обрабатывает включение чекбокса тёмного режима на вкладке настроек.</summary>
    private void ChkDarkMode_Checked(object sender, RoutedEventArgs e) =>
        MenuDarkTheme_Click(sender, e);

    /// <summary>Обрабатывает выключение чекбокса тёмного режима на вкладке настроек.</summary>
    private void ChkDarkMode_Unchecked(object sender, RoutedEventArgs e) =>
        MenuLightTheme_Click(sender, e);

    /// <summary>Обновляет строку состояния при переключении между вкладками.</summary>
    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (statusText is null) return;
        string tab = (mainTabControl.SelectedItem as TabItem)?.Header?.ToString()?.Trim() ?? "";
        int completed = Habits.Count(h => h.IsCompleted);
        statusText.Text = $"Вкладка: «{tab}» | Привычек: {Habits.Count} (выполнено: {completed})";
    }
}
