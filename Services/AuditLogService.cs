using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditRepository _repository;

    public AuditLogService(
        IAuditRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AuditLog>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedAsync(
            page,
            pageSize,
            search);
    }
}