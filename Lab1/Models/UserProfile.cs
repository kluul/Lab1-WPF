namespace Lab1.Models;

public class UserProfile
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string Education { get; set; } = string.Empty;
    public List<string> Hobbies { get; set; } = new();
    public bool ReceiveNotifications { get; set; }
    public bool ShowPublicStats { get; set; }
    public bool AutoSave { get; set; } = true;
    public string ActivityLevel { get; set; } = "Средний";
    public string AvatarPath { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
}
