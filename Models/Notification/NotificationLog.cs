namespace StudentManagementSystemApp.Models.Notification;

public class NotificationLog
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public string StudentName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Subject { get; set; } = "";

    public string? Body { get; set; }

    public string NotificationType { get; set; } = "";

    public string Status { get; set; } = "";

    public string? ErrorMessage { get; set; }

    public DateTime SentDate { get; set; }
}