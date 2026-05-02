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

public partial class MainWindow : Window
{
    private readonly UserProfile profile = new();
    public ObservableCollection<Habit> Habits { get; } = HabitService.CreateSampleHabits();
    private readonly ObservableCollection<DayStatistic> statistics = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        lvStatistics.ItemsSource = statistics;
        InitializeStatistics();
        UpdateStatus();
        statusDate.Text = DateTime.Now.ToShortDateString();
    }

    private void InitializeStatistics()
    {
        foreach (var stat in StatisticsService.GenerateSampleStatistics(days: 7))
            statistics.Add(stat);
    }

    private void UpdateStatus()
    {
        int completed = Habits.Count(h => h.IsCompleted);
        statusText.Text = $"Готово | Привычек: {Habits.Count} (выполнено: {completed})";
        statusUser.Text = string.IsNullOrEmpty(profile.FullName)
            ? "Пользователь не задан"
            : profile.FullName;
    }

    private void UpdateDayProgress()
    {
        if (pbDayProgress is null || slProductivity is null || slSatisfaction is null) return;
        pbDayProgress.Value = (slProductivity.Value + slSatisfaction.Value) / 2.0;
    }

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

    private void SetValidationError(string message)
    {
        lblValidation.Text = $"⚠ {message}";
        lblValidation.Foreground = Brushes.Red;
    }

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

    private void SlProductivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (lblProductivity is null) return;
        lblProductivity.Text = $"{(int)slProductivity.Value}%";
        UpdateDayProgress();
    }

    private void SlSatisfaction_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (lblSatisfaction is null) return;
        lblSatisfaction.Text = $"{(int)slSatisfaction.Value}%";
        UpdateDayProgress();
    }

    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (calendar.SelectedDate.HasValue)
            statusDate.Text = calendar.SelectedDate.Value.ToShortDateString();
        UpdateStatus();
    }

    private void DgHabits_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        Dispatcher.InvokeAsync(UpdateStatus);
    }

    private void SaveTodayStatistic()
    {
        var todayStat = HabitService.BuildTodayStatistic(
            Habits, slProductivity.Value, slSatisfaction.Value);

        var existing = statistics.FirstOrDefault(s => s.Date == DateTime.Today);
        if (existing is not null)
            statistics.Remove(existing);

        statistics.Add(todayStat);
    }

    private void RepeatUp_Click(object sender, RoutedEventArgs e)
    {
        if (slProductivity.Value < slProductivity.Maximum)
            slProductivity.Value++;
    }

    private void RepeatDown_Click(object sender, RoutedEventArgs e)
    {
        if (slProductivity.Value > slProductivity.Minimum)
            slProductivity.Value--;
    }

    private void ToggleEditMode_Checked(object sender, RoutedEventArgs e)
    {
        extraPanel.Visibility = Visibility.Visible;
        UpdateStatus();
    }

    private void ToggleEditMode_Unchecked(object sender, RoutedEventArgs e)
    {
        extraPanel.Visibility = Visibility.Collapsed;
        UpdateStatus();
    }

    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile_Click(sender, e);
        if (string.IsNullOrEmpty(lblValidation.Text) || lblValidation.Foreground == Brushes.Green)
            MessageBox.Show("Данные профиля сохранены!", "Сохранение",
                MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuLoad_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Функция загрузки из файла будет реализована в следующей версии.",
            "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e) =>
        Application.Current.Shutdown();

    private void MenuCopy_Click(object sender, RoutedEventArgs e) =>
        ApplicationCommands.Copy.Execute(null, FocusManager.GetFocusedElement(this) as IInputElement);

    private void MenuPaste_Click(object sender, RoutedEventArgs e) =>
        ApplicationCommands.Paste.Execute(null, FocusManager.GetFocusedElement(this) as IInputElement);

    private void MenuDarkTheme_Click(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/DarkTheme.xaml");

    private void MenuLightTheme_Click(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/LightTheme.xaml");

    private void ChkDarkMode_Checked(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/DarkTheme.xaml");

    private void ChkDarkMode_Unchecked(object sender, RoutedEventArgs e) =>
        ApplyTheme("Themes/LightTheme.xaml");

    private static void ApplyTheme(string themeUri)
    {
        var dict = new ResourceDictionary
        {
            Source = new Uri(themeUri, UriKind.Relative)
        };

        var existing = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") == true);

        if (existing is not null)
            Application.Current.Resources.MergedDictionaries.Remove(existing);

        Application.Current.Resources.MergedDictionaries.Add(dict);
    }

    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (statusText is null) return;
        string tab = (mainTabControl.SelectedItem as TabItem)?.Header?.ToString()?.Trim() ?? "";
        int completed = Habits.Count(h => h.IsCompleted);
        statusText.Text = $"Вкладка: «{tab}» | Привычек: {Habits.Count} (выполнено: {completed})";
    }
}
