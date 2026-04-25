namespace Lab1.Validation;

/// <summary>Результат валидации профиля пользователя.</summary>
public sealed class ValidationResult
{
    /// <summary>Признак успешной валидации.</summary>
    public bool IsValid { get; init; }

    /// <summary>Сообщение об ошибке. Пустая строка при успехе.</summary>
    public string ErrorMessage { get; init; } = string.Empty;

    /// <summary>Возвращает успешный результат без ошибок.</summary>
    public static ValidationResult Ok() => new() { IsValid = true };

    /// <summary>Возвращает результат с ошибкой.</summary>
    /// <param name="message">Текст ошибки для отображения пользователю.</param>
    public static ValidationResult Fail(string message) =>
        new() { IsValid = false, ErrorMessage = message };
}

/// <summary>Набор правил валидации данных профиля пользователя.</summary>
public static class ProfileValidator
{
    /// <summary>Проверяет, что имя не пустое и содержит только буквы.</summary>
    /// <param name="firstName">Значение поля «Имя».</param>
    public static ValidationResult ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return ValidationResult.Fail("Введите имя.");
        if (!firstName.All(c => char.IsLetter(c) || c == '-' || c == ' '))
            return ValidationResult.Fail("Имя должно содержать только буквы.");
        return ValidationResult.Ok();
    }

    /// <summary>Проверяет, что фамилия не пустая и содержит только буквы.</summary>
    /// <param name="lastName">Значение поля «Фамилия».</param>
    public static ValidationResult ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return ValidationResult.Fail("Введите фамилию.");
        if (!lastName.All(c => char.IsLetter(c) || c == '-' || c == ' '))
            return ValidationResult.Fail("Фамилия должна содержать только буквы.");
        return ValidationResult.Ok();
    }

    /// <summary>Проверяет, что дата рождения не находится в будущем.</summary>
    /// <param name="birthDate">Выбранная дата рождения или null если не задана.</param>
    public static ValidationResult ValidateBirthDate(DateTime? birthDate)
    {
        if (birthDate.HasValue && birthDate.Value > DateTime.Today)
            return ValidationResult.Fail("Дата рождения не может быть в будущем.");
        return ValidationResult.Ok();
    }

    /// <summary>Последовательно применяет все правила валидации профиля.</summary>
    /// <param name="firstName">Имя пользователя.</param>
    /// <param name="lastName">Фамилия пользователя.</param>
    /// <param name="birthDate">Дата рождения пользователя.</param>
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
