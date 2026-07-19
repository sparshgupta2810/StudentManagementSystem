using System.Text.Json;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditComparer _auditComparer;

    public AuditService(
        IAuditRepository repository,
        ICurrentUserService currentUserService,
        IAuditComparer auditComparer)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _auditComparer = auditComparer;
    }

    public async Task LogAsync(
        AuditModule module,
        AuditAction action,
        object entityId,
        string entityName,
        string? details = null,
        object? oldValue = null,
        object? newValue = null)
    {
        var currentUser =
            await _currentUserService.GetCurrentUserAsync();

        if (!currentUser.IsAuthenticated)
            return;

        var changes = _auditComparer.Compare(
            oldValue,
            newValue);

        var audit = new AuditLog
        {
            UserId = currentUser.UserId,
            UserName = currentUser.UserName,
            FullName = currentUser.FullName,
            Email = currentUser.Email,
            RoleName = currentUser.Role,

            ModuleName = module.ToString(),
            ActionName = action.ToString(),

            EntityId = entityId.ToString()!,
            EntityName = entityName,

            Details = details,

            // Store comparison result
            OldValue = JsonSerializer.Serialize(changes),

            // Keep same JSON for now
            NewValue = JsonSerializer.Serialize(changes),

            CreatedDate = DateTime.Now
        };

        await _repository.AddAsync(audit);
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