using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(AuditLog log);

    Task<PagedResult<AuditLog>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null);
}