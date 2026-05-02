namespace Lab1.Validation;

// Результат валидации профиля пользователя
public sealed class ValidationResult
{
    // Признак успешной валидации
    public bool IsValid { get; init; }

    // Сообщение об ошибке, пустая строка при успехе
    public string ErrorMessage { get; init; } = string.Empty;

    // Возвращает успешный результат без ошибок
    public static ValidationResult Ok() => new() { IsValid = true };

    // Возвращает результат с ошибкой
    public static ValidationResult Fail(string message) =>
        new() { IsValid = false, ErrorMessage = message };
}

// Набор правил валидации данных профиля пользователя
public static class ProfileValidator
{
    // Проверяет, что имя не пустое и содержит только буквы
    public static ValidationResult ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return ValidationResult.Fail("Введите имя.");
        if (!firstName.All(c => char.IsLetter(c) || c == '-' || c == ' '))
            return ValidationResult.Fail("Имя должно содержать только буквы.");
        return ValidationResult.Ok();
    }

    // Проверяет, что фамилия не пустая и содержит только буквы
    public static ValidationResult ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return ValidationResult.Fail("Введите фамилию.");
        if (!lastName.All(c => char.IsLetter(c) || c == '-' || c == ' '))
            return ValidationResult.Fail("Фамилия должна содержать только буквы.");
        return ValidationResult.Ok();
    }

    // Проверяет, что дата рождения не находится в будущем
    public static ValidationResult ValidateBirthDate(DateTime? birthDate)
    {
        if (birthDate.HasValue && birthDate.Value > DateTime.Today)
            return ValidationResult.Fail("Дата рождения не может быть в будущем.");
        return ValidationResult.Ok();
    }

    // Последовательно применяет все правила валидации профиля
    public static ValidationResult ValidateAll(
        string firstName, string lastName, DateTime? birthDate)
    {
        var checks = new[]
        {
            ValidateFirstName(firstName),
            ValidateLastName(lastName),
            ValidateBirthDate(birthDate)
        };

        return checks.FirstOrDefault(r => !r.IsValid) ?? ValidationResult.Ok();
    }
}
