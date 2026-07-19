using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IAuditLogService
{
    Task<PagedResult<AuditLog>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null);
}