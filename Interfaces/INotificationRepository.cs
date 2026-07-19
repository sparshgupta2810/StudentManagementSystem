using StudentManagementSystemApp.Models;
using StudentManagementSystemApp.Models.Notification;

namespace StudentManagementSystemApp.Interfaces;

public interface INotificationRepository
{
    Task AddLogAsync(NotificationLog log);

    Task<IEnumerable<NotificationLog>> GetLogsAsync();

    Task<PagedResult<NotificationLog>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null);
}