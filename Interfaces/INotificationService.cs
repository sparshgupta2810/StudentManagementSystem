namespace StudentManagementSystemApp.Interfaces;

public interface INotificationService
{
    Task SendBookIssuedAsync(BookIssue issue);

    Task SendBookReturnedAsync(BookIssue issue);

    Task SendOverdueReminderAsync(BookIssue issue);
}