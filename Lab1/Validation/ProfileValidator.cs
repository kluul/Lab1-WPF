namespace Lab1.Validation;

public sealed class ValidationResult
{
    public bool IsValid { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;

    public static ValidationResult Ok() => new() { IsValid = true };

    public static ValidationResult Fail(string message) =>
        new() { IsValid = false, ErrorMessage = message };
}

public static class ProfileValidator
{
    public static ValidationResult ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return ValidationResult.Fail("Введите имя.");
        if (!firstName.All(c => char.IsLetter(c) || c == '-' || c == ' '))
            return ValidationResult.Fail("Имя должно содержать только буквы.");
        return ValidationResult.Ok();
    }

    public static ValidationResult ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return ValidationResult.Fail("Введите фамилию.");
        if (!lastName.All(c => char.IsLetter(c) || c == '-' || c == ' '))
            return ValidationResult.Fail("Фамилия должна содержать только буквы.");
        return ValidationResult.Ok();
    }

    public static ValidationResult ValidateBirthDate(DateTime? birthDate)
    {
        if (birthDate.HasValue && birthDate.Value > DateTime.Today)
            return ValidationResult.Fail("Дата рождения не может быть в будущем.");
        return ValidationResult.Ok();
    }

    public static ValidationResult ValidateAll(string firstName, string lastName, DateTime? birthDate)
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
