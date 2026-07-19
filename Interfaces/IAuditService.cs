using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        AuditModule module,
        AuditAction action,
        object entityId,
        string entityName,
        string? details = null,
        object? oldValue = null,
        object? newValue = null);

    Task<PagedResult<AuditLog>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null);
}