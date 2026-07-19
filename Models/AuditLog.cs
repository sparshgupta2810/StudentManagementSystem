namespace StudentManagementSystemApp.Models;

public class AuditLog
{
    public int Id { get; set; }

    // User who performed the action
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    // Module
    public string ModuleName { get; set; } = string.Empty;

    // Add / Update / Delete / Restore
    public string ActionName { get; set; } = string.Empty;

    // Entity affected
    public string EntityId { get; set; } = string.Empty;

    public string? EntityName { get; set; }

    // Message shown in audit page
    public string? Details { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime CreatedDate { get; set; }
}