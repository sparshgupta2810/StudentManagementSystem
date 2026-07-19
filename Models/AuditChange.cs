namespace StudentManagementSystemApp.Models;

public class AuditChange
{
    public string Property { get; set; } = string.Empty;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
}